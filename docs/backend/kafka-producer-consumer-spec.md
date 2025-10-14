## Kafka Integration - Producer and Consumer Specification

### Goals
- Standardize event topics, payload schema, logging, and error handling.
- Enable a minimal Producer now; add the Consumer next.

### Topics
- port-events
- container-events
- Dead-letter topics (DLQ):
  - port-events.dlq
  - container-events.dlq

### Event Payload Schema (JSON)
All events must follow this shape; omit null fields.
```
{
  "id": number,                // internal EventId
  "type": string,              // e.g., "ArrivedAtPort", "InspectionCompleted", "Loaded", "Departed"
  "status": string,            // e.g., "New", "Acknowledged", "Resolved"
  "timestamp": string,         // ISO-8601 UTC
  "source": string,            // subsystem or service name
  "portId": number,
  "shipId": number,
  "containerId": number,
  "berthId": number,
  "priority": string,          // Critical|High|Medium|Low
  "requiresAction": boolean
}
```

Key selection:
- Use a stable key for ordering: container-events → containerId (as string); port-events → portId (as string). If absent, use `id`.

### Producer Responsibilities
1. Build payload from `Backend.Models.Event` after DB `SaveChangesAsync` in `EventService.CreateAsync`.
2. Pick topic by category/type:
   - Category/Type relates to `container-events` vs `port-events`. If unclear, default to `port-events`.
3. Publish with key + payload JSON.
4. Logging on success: topic, key, partition, offset, id.
5. Logging on error and surface exception to caller (API still returns success if DB write succeeded; publish error is logged).

### Consumer Responsibilities (phase 2)
1. Subscribe to `port-events`, `container-events`.
2. Process messages (for MVP, log receipt; optionally persist to a `ReceivedEvents` table or push to a live hub).
3. Error handling: retry up to 3 times with backoff (e.g., 100ms, 500ms, 2s). On permanent failure, publish original payload + error to corresponding `*.dlq` topic and log an error with context.
4. Ensure service does not crash on failures.

### Error Handling & DLQ
- Retries: bounded attempts, log attempt count.
- DLQ payload must include:
```
{
  "originalTopic": string,
  "originalKey": string,
  "payload": object,           // original event payload
  "error": string,
  "failedAt": string          // ISO-8601 UTC
}
```

### Configuration (appsettings.*)
```
"Kafka": {
  "BootstrapServers": "localhost:9092",
  "ConsumerGroupId": "container-tracking-group",
  "Topics": {
    "PortEvents": "port-events",
    "ContainerEvents": "container-events"
  },
  "EnableDlq": true
}
```

### Local Setup (Docker)
1. Requirements: Docker Desktop installed and running.
2. Use a simple docker-compose (KRaft or Zookeeper). Example (KRaft) snippet:
```
version: "3.8"
services:
  kafka:
    image: bitnami/kafka:latest
    environment:
      - KAFKA_CFG_PROCESS_ROLES=broker,controller
      - KAFKA_CFG_NODE_ID=1
      - KAFKA_CFG_CONTROLLER_QUORUM_VOTERS=1@kafka:9093
      - KAFKA_CFG_LISTENERS=PLAINTEXT://:9092,CONTROLLER://:9093
      - KAFKA_CFG_ADVERTISED_LISTENERS=PLAINTEXT://localhost:9092
      - KAFKA_CFG_LISTENER_SECURITY_PROTOCOL_MAP=CONTROLLER:PLAINTEXT,PLAINTEXT:PLAINTEXT
      - KAFKA_CFG_INTER_BROKER_LISTENER_NAME=PLAINTEXT
      - KAFKA_CFG_CONTROLLER_LISTENER_NAMES=CONTROLLER
    ports:
      - "9092:9092"
```
3. Create topics (optional; Kafka can auto-create if enabled): `port-events`, `container-events`, `port-events.dlq`, `container-events.dlq`.

### .NET Dependencies
- Add NuGet package: `Confluent.Kafka`.

### Wiring (Producer - MVP)
- Register in `Program.cs`:
```
builder.Services.AddSingleton<IKafkaProducer, KafkaProducerService>();
```
- After persisting event in `EventService.CreateAsync`, call:
```
await _kafkaProducer.PublishAsync(topic, key, JsonSerializer.Serialize(payload));
```

### Logging (evidence for submission)
- Producer: `Published event {EventId} to {Topic} key={Key} partition={Partition} offset={Offset}`.
- Consumer: `Consumed event from {Topic} partition={Partition} offset={Offset} key={Key}`.
- On failure: `Publishing failed for {Topic} key={Key}: {Error}`. For DLQ: `Routed failed message to {DlqTopic}`.

### Submission Checklist (Producer)
- [ ] `Confluent.Kafka` added.
- [ ] Appsettings contains Kafka section.
- [ ] `IKafkaProducer` + `KafkaProducerService` implemented with logging.
- [ ] Producer wired into `EventService.CreateAsync`.
- [ ] README/doc includes how to run Kafka locally and sample logs.

### Submission Checklist (Consumer - next)
- [ ] BackgroundService consumer subscribed to topics.
- [ ] Retry + DLQ implemented.
- [ ] Logs of consumed messages.


