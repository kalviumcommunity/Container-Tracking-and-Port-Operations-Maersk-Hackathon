using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Backend.Services.Kafka;
using Backend.DTOs;
using Backend.Hubs;
using Backend.Services;

namespace Backend.Services.Kafka
{
    /// <summary>
    /// Background service that consumes events from Kafka topics and processes them
    /// </summary>
    public class KafkaConsumerService : BackgroundService
    {
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly KafkaSettings _kafkaSettings;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConsumer<string, string> _consumer;
        private readonly List<string> _topics;

        public KafkaConsumerService(
            IOptions<KafkaSettings> kafkaSettings,
            IServiceProvider serviceProvider,
            ILogger<KafkaConsumerService> logger)
        {
            _logger = logger;
            _kafkaSettings = kafkaSettings.Value;
            _serviceProvider = serviceProvider;

            // Configure consumer
            var config = new ConsumerConfig
            {
                BootstrapServers = _kafkaSettings.BootstrapServers,
                GroupId = _kafkaSettings.ConsumerGroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false, // Manual commit for better control
                EnableAutoOffsetStore = false,
                SessionTimeoutMs = 6000,
                MaxPollIntervalMs = 300000,
                AllowAutoCreateTopics = true
            };

            _consumer = new ConsumerBuilder<string, string>(config)
                .SetErrorHandler((_, e) => _logger.LogError("Kafka consumer error: {Reason}", e.Reason))
                .SetPartitionsAssignedHandler((c, partitions) =>
                {
                    _logger.LogInformation("Partitions assigned: {Partitions}", 
                        string.Join(", ", partitions.Select(p => $"{p.Topic}[{p.Partition}]")));
                })
                .SetPartitionsRevokedHandler((c, partitions) =>
                {
                    _logger.LogInformation("Partitions revoked: {Partitions}", 
                        string.Join(", ", partitions.Select(p => $"{p.Topic}[{p.Partition}]")));
                })
                .Build();

            // Subscribe to all configured topics
            _topics = new List<string>
            {
                _kafkaSettings.Topics.PortEvents,
                _kafkaSettings.Topics.ContainerEvents
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // CRITICAL: Delay startup to allow web server to bind first
            // This prevents the consumer from blocking Kestrel initialization
            _logger.LogInformation("⏳ Kafka Consumer Service: Waiting 5 seconds for web server to start...");
            
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Kafka Consumer Service startup canceled");
                return;
            }

            _logger.LogInformation("Kafka Consumer Service starting. Subscribing to topics: {Topics}", 
                string.Join(", ", _topics));

            try
            {
                // Try to subscribe to topics
                _consumer.Subscribe(_topics);
                _logger.LogInformation("✅ Successfully subscribed to Kafka topics");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to subscribe to Kafka topics. Consumer will retry in background. Web server will continue.");
                
                // Don't crash the app - allow web server to start
                // Retry connection in background
                _ = Task.Run(async () =>
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                            _logger.LogInformation("🔄 Attempting to reconnect to Kafka...");
                            _consumer.Subscribe(_topics);
                            _logger.LogInformation("✅ Kafka reconnected successfully!");
                            break;
                        }
                        catch (Exception retryEx)
                        {
                            _logger.LogWarning(retryEx, "Kafka reconnection attempt failed. Will retry in 30s.");
                        }
                    }
                }, stoppingToken);
                
                return; // Exit gracefully without crashing
            }

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        // Poll for messages (blocking call with timeout)
                        var consumeResult = _consumer.Consume(TimeSpan.FromSeconds(1));

                        if (consumeResult != null && consumeResult.Message != null)
                        {
                            await ProcessMessageAsync(consumeResult, stoppingToken);

                            // Store offset and commit after successful processing
                            _consumer.StoreOffset(consumeResult);
                            _consumer.Commit(consumeResult);
                        }
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "Consume error: {Error}", ex.Error.Reason);
                        
                        // If it's a fatal error, break the loop
                        if (ex.Error.IsFatal)
                        {
                            _logger.LogCritical("Fatal Kafka consumer error. Stopping consumer.");
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing Kafka message");
                        // Continue processing other messages
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Kafka Consumer Service is stopping due to cancellation");
            }
            finally
            {
                try
                {
                    _consumer.Close();
                    _logger.LogInformation("Kafka Consumer Service stopped");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error closing Kafka consumer (this is OK during shutdown)");
                }
            }
        }

        /// <summary>
        /// Process a consumed Kafka message
        /// </summary>
        private async Task ProcessMessageAsync(ConsumeResult<string, string> result, CancellationToken cancellationToken)
        {
            var topic = result.Topic;
            var key = result.Message.Key;
            var value = result.Message.Value;
            var partition = result.Partition.Value;
            var offset = result.Offset.Value;

            _logger.LogInformation(
                "Received message: Topic={Topic}, Partition={Partition}, Offset={Offset}, Key={Key}",
                topic, partition, offset, key);

            try
            {
                // Deserialize the event
                var eventDto = JsonSerializer.Deserialize<EventDto>(value, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (eventDto == null)
                {
                    _logger.LogWarning("Failed to deserialize event from topic {Topic}, offset {Offset}", 
                        topic, offset);
                    return;
                }

                // Process the event based on topic
                if (topic == _kafkaSettings.Topics.PortEvents)
                {
                    await ProcessPortEventAsync(eventDto, cancellationToken);
                }
                else if (topic == _kafkaSettings.Topics.ContainerEvents)
                {
                    await ProcessContainerEventAsync(eventDto, cancellationToken);
                }
                else
                {
                    _logger.LogWarning("Received message from unknown topic: {Topic}", topic);
                }

                // Broadcast to SignalR clients
                await BroadcastEventToClientsAsync(eventDto, cancellationToken);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize event JSON: {Value}", value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing event from topic {Topic}", topic);
            }
        }

        /// <summary>
        /// Process port-related events
        /// </summary>
        private async Task ProcessPortEventAsync(EventDto eventDto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing Port Event: {EventType} - {Title}", 
                eventDto.EventType, eventDto.Title);

            using var scope = _serviceProvider.CreateScope();
            
            // Send email notification for high-priority events
            try
            {
                var emailService = scope.ServiceProvider.GetService<IEmailService>();
                if (emailService != null)
                {
                    if (eventDto.Severity == "Critical")
                    {
                        await emailService.SendCriticalAlertAsync(eventDto);
                    }
                    else if (eventDto.Severity == "High")
                    {
                        await emailService.SendEventNotificationAsync(eventDto);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email notification for port event {EventId}", eventDto.EventId);
            }
        }

        /// <summary>
        /// Process container-related events
        /// </summary>
        private async Task ProcessContainerEventAsync(EventDto eventDto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing Container Event: {EventType} - {Title}", 
                eventDto.EventType, eventDto.Title);

            using var scope = _serviceProvider.CreateScope();
            
            // Send email notification for high-priority events
            try
            {
                var emailService = scope.ServiceProvider.GetService<IEmailService>();
                if (emailService != null)
                {
                    if (eventDto.Severity == "Critical")
                    {
                        await emailService.SendCriticalAlertAsync(eventDto);
                    }
                    else if (eventDto.Severity == "High")
                    {
                        await emailService.SendEventNotificationAsync(eventDto);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email notification for container event {EventId}", eventDto.EventId);
            }
        }

        /// <summary>
        /// Broadcast event to connected SignalR clients
        /// </summary>
        private async Task BroadcastEventToClientsAsync(EventDto eventDto, CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var eventHubService = scope.ServiceProvider.GetRequiredService<IEventHubService>();

                // Broadcast to all clients
                await eventHubService.BroadcastEventAsync(eventDto);

                // Also broadcast to category-specific groups
                if (!string.IsNullOrEmpty(eventDto.Category))
                {
                    await eventHubService.BroadcastEventToCategoryAsync(eventDto.Category, eventDto);
                }

                // Also broadcast to severity-specific groups
                if (!string.IsNullOrEmpty(eventDto.Severity))
                {
                    await eventHubService.BroadcastEventToSeverityAsync(eventDto.Severity, eventDto);
                }

                _logger.LogDebug("Event {EventId} broadcasted to SignalR clients", eventDto.EventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to broadcast event {EventId} to SignalR clients", eventDto.EventId);
            }
        }

        /// <summary>
        /// Cleanup when the service is stopped
        /// </summary>
        public override void Dispose()
        {
            try
            {
                _consumer?.Close();
                _logger.LogInformation("Kafka consumer closed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error closing Kafka consumer during dispose (this is OK during shutdown)");
            }

            try
            {
                _consumer?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error disposing Kafka consumer (this is OK during shutdown)");
            }

            base.Dispose();
        }
    }
}
