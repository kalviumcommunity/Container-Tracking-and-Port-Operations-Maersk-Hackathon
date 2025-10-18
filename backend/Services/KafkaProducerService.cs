using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Backend.Services.Kafka
{
    public interface IKafkaProducer
    {
        Task PublishAsync(string topic, string key, string value, CancellationToken cancellationToken = default);
    }

    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } = string.Empty;
        public string ConsumerGroupId { get; set; } = "container-tracking-group";
        public TopicsSettings Topics { get; set; } = new TopicsSettings();
        public bool EnableDlq { get; set; } = true;
        public bool UseEventHubs { get; set; } = false;
        public string EventHubsConnectionString { get; set; } = string.Empty;
    }

    public class TopicsSettings
    {
        public string PortEvents { get; set; } = "port-events";
        public string ContainerEvents { get; set; } = "container-events";
    }

    public class KafkaProducerService : IKafkaProducer, IDisposable
    {
        private readonly ILogger<KafkaProducerService> _logger;
        private readonly IProducer<string, string> _producer;
        private readonly KafkaSettings _settings;

        public KafkaProducerService(IOptions<KafkaSettings> settings, ILogger<KafkaProducerService> logger)
        {
            _logger = logger;
            _settings = settings.Value;

            var config = new ProducerConfig
            {
                BootstrapServers = _settings.BootstrapServers,
                Acks = Acks.All,
                EnableIdempotence = true,
                MessageSendMaxRetries = 3,
                RetryBackoffMs = 200
            };

            // Azure Event Hubs configuration
            if (_settings.UseEventHubs && !string.IsNullOrEmpty(_settings.EventHubsConnectionString))
            {
                config.SecurityProtocol = SecurityProtocol.SaslSsl;
                config.SaslMechanism = SaslMechanism.Plain;
                config.SaslUsername = "$ConnectionString";
                config.SaslPassword = _settings.EventHubsConnectionString;
                
                _logger.LogInformation("Kafka producer configured for Azure Event Hubs");
            }

            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task PublishAsync(string topic, string key, string value, CancellationToken cancellationToken = default)
        {
            try
            {
                var message = new Message<string, string>
                {
                    Key = key,
                    Value = value
                };

                var result = await _producer.ProduceAsync(topic, message, cancellationToken);

                _logger.LogInformation(
                    "Published event to topic={Topic} key={Key} partition={Partition} offset={Offset}",
                    result.Topic, result.Key, result.Partition, result.Offset);
            }
            catch (ProduceException<string, string> ex)
            {
                _logger.LogError(ex, "Kafka publish failed for topic={Topic} key={Key}: {Reason}", topic, key, ex.Error.Reason);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kafka publish failed for topic={Topic} key={Key}", topic, key);
                throw;
            }
        }

        public void Dispose()
        {
            try
            {
                _producer.Flush(TimeSpan.FromSeconds(2));
                _producer.Dispose();
            }
            catch
            {
            }
        }
    }
}


