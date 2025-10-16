# 🚀 Quick Start: Running Kafka Locally

## Prerequisites
- ✅ Docker Desktop installed and running
- ✅ PowerShell/Terminal access

## Start Kafka (2 commands)

```powershell
# 1. Start Kafka + Zookeeper
docker-compose -f docker-compose.kafka.yml up -d

# 2. Verify it's running
docker-compose -f docker-compose.kafka.yml ps
```

## Test It Works (3 steps)

```powershell
# 1. List topics
docker exec kafka kafka-topics --list --bootstrap-server localhost:9092

# 2. Create a test event (via Postman or curl)
POST http://localhost:5000/api/events
{
  "eventType": "test",
  "title": "Test Event",
  "description": "Testing Kafka",
  "eventTime": "2025-01-16T10:00:00Z",
  "severity": "low"
}

# 3. Check if message arrived
docker exec kafka kafka-console-consumer --bootstrap-server localhost:9092 --topic port-events --from-beginning --max-messages 1
```

## Stop Kafka

```powershell
# Stop (keeps data)
docker-compose -f docker-compose.kafka.yml stop

# Or completely remove
docker-compose -f docker-compose.kafka.yml down
```

## Common Commands

```powershell
# View Kafka logs
docker logs kafka

# List all topics
docker exec kafka kafka-topics --list --bootstrap-server localhost:9092

# Watch messages in real-time
docker exec -it kafka kafka-console-consumer --bootstrap-server localhost:9092 --topic container-events --from-beginning

# Check if port 9092 is in use
netstat -ano | findstr :9092
```

## Troubleshooting

**Port already in use?**
```powershell
# Find process using port 9092
netstat -ano | findstr :9092

# Kill it or change port in docker-compose.kafka.yml
```

**Can't connect?**
```powershell
# Check Kafka is accessible
docker exec kafka kafka-broker-api-versions --bootstrap-server localhost:9092
```

**No messages appearing?**
```powershell
# Check backend logs for errors
# Verify: backend/appsettings.Development.json has:
# "BootstrapServers": "localhost:9092"
```

---

## Full Documentation
See: `docs/KAFKA_STATUS_REPORT.md`
