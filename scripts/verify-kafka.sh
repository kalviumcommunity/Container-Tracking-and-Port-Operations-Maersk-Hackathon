#!/usr/bin/env bash
set -euo pipefail

# Quick verification script for local Kafka + Backend publishing
# Usage: ./scripts/verify-kafka.sh [BROKER] [BACKEND_BASE_URL]
# Example: ./scripts/verify-kafka.sh localhost:9092 http://localhost:5221

BROKER="${1:-${KAFKA_BOOTSTRAP_SERVERS:-localhost:9092}}"
BACKEND="${2:-http://localhost:5221}"
TEST_TOPIC="porttrack-test"
CONTAINER_TOPIC="${KAFKA_TOPIC_CONTAINER_EVENTS:-container-events}"

echo "Broker: $BROKER"
echo "Backend: $BACKEND"
echo ""

# Find running kafka container (tries common image names)
KAFKA_CONTAINER=$(docker ps -qf "name=kafka" || true)
if [ -z "$KAFKA_CONTAINER" ]; then
  KAFKA_CONTAINER=$(docker ps -qf "ancestor=bitnami/kafka" || true)
fi
if [ -z "$KAFKA_CONTAINER" ]; then
  KAFKA_CONTAINER=$(docker ps -qf "ancestor=confluentinc/cp-kafka" || true)
fi
if [ -z "$KAFKA_CONTAINER" ]; then
  KAFKA_CONTAINER=$(docker ps -qf "ancestor=wurstmeister/kafka" || true)
fi

echo "1) Check Kafka broker TCP reachability..."
HOST="${BROKER%%:*}"
PORT="${BROKER#*:}"
if command -v nc >/dev/null 2>&1; then
  if nc -zv "$HOST" "$PORT"; then
    echo "   OK: $BROKER reachable"
  else
    echo "   WARNING: $BROKER not reachable (nc check failed)"
  fi
else
  echo "   nc not installed — please ensure $BROKER is reachable (or install 'nc')"
fi
echo

echo "2) List topics (prefer docker kafka-tools or kcat)..."

# Helper to find CLI inside container
find_in_container() {
  local container="$1"
  shift
  for p in "$@"; do
    if docker exec -i "$container" bash -lc "test -x '$p' && echo '$p' && exit 0" >/dev/null 2>&1; then
      echo "$p"
      return 0
    fi
  done
  return 1
}

TOPICS_CMD=""
PRODUCER_CMD=""
CONSUMER_CMD=""

if [ -n "$KAFKA_CONTAINER" ]; then
  echo "   Using docker container $KAFKA_CONTAINER to find kafka CLI..."
  # Common candidate paths
  CANDIDATES=(
    "/opt/bitnami/kafka/bin/kafka-topics.sh"
    "/opt/bitnami/kafka/bin/kafka-console-producer.sh"
    "/opt/bitnami/kafka/bin/kafka-console-consumer.sh"
    "/usr/bin/kafka-topics.sh"
    "/usr/local/bin/kafka-topics.sh"
    "/opt/confluent/bin/kafka-topics"
    "/opt/confluent/bin/kafka-console-producer"
    "/opt/confluent/bin/kafka-console-consumer"
    "/kafka/bin/kafka-topics.sh"
    "/kafka/bin/kafka-console-producer.sh"
    "/kafka/bin/kafka-console-consumer.sh"
    "/bin/kafka-topics.sh"
  )

  TOPICS_CANDIDATE=$(find_in_container "$KAFKA_CONTAINER" "${CANDIDATES[@]}" || true)

  if [ -n "$TOPICS_CANDIDATE" ]; then
    # Derive producer/consumer paths based on found root
    TOPICS_CMD="$TOPICS_CANDIDATE"
    # Try to infer producer/consumer by replacing filename
    if echo "$TOPICS_CMD" | grep -q "kafka-topics"; then
      PRODUCER_CMD="${TOPICS_CMD//kafka-topics/kafka-console-producer}"
      CONSUMER_CMD="${TOPICS_CMD//kafka-topics/kafka-console-consumer}"
    fi

    echo "   Found kafka CLI: $TOPICS_CMD"
    echo "   Producer: $PRODUCER_CMD"
    echo "   Consumer: $CONSUMER_CMD"

    echo "   Listing topics via container..."
    docker exec -i "$KAFKA_CONTAINER" bash -lc "$TOPICS_CMD --bootstrap-server localhost:9092 --list" || true
  else
    echo "   kafka CLI not found inside container."
  fi
else
  echo "   No kafka container detected."
fi

# If no container tools, try kcat locally
if command -v kcat >/dev/null 2>&1; then
  echo ""
  echo "   kcat is available locally. Listing cluster metadata..."
  kcat -L -b "$BROKER" || true
fi

echo ""
echo "3) Create test topic: $TEST_TOPIC (if not exists)"
if [ -n "$TOPICS_CMD" ] && [ -n "$KAFKA_CONTAINER" ]; then
  docker exec -i "$KAFKA_CONTAINER" bash -lc "$TOPICS_CMD --bootstrap-server localhost:9092 --create --topic $TEST_TOPIC --partitions 1 --replication-factor 1 || true"
  echo "   Topic create attempted inside container"
else
  echo "   Skipping create: kafka CLI not available in container."
  echo "   Alternative: use kafka-ui (http://localhost:8080) or install 'kcat' / Kafka CLI tools locally."
fi
echo

echo "4) Produce a test message to $TEST_TOPIC..."
PAYLOAD="{\"msg\":\"hello\",\"ts\":\"$(date -u +"%Y-%m-%dT%H:%M:%SZ")\"}"

produced=false
if command -v kcat >/dev/null 2>&1; then
  echo "$PAYLOAD" | kcat -P -b "$BROKER" -t "$TEST_TOPIC" && produced=true || true
  if $produced; then echo "   Produced with kcat"; fi
fi

if ! $produced && [ -n "$PRODUCER_CMD" ] && [ -n "$KAFKA_CONTAINER" ]; then
  # Use docker exec producer (pipe payload into producer)
  docker exec -i "$KAFKA_CONTAINER" bash -lc "cat > /tmp/msg.json && $PRODUCER_CMD --broker-list localhost:9092 --topic $TEST_TOPIC < /tmp/msg.json" <<EOF
$PAYLOAD
EOF
  produced=true && echo "   Produced via container producer"
fi

if ! $produced; then
  echo "   Cannot produce message: no kcat and no producer CLI found in container."
  echo "   To fix: install kcat locally or run a kafka-tools container with network access to the broker."
fi
echo

echo "5) Consume the test message from $TEST_TOPIC (timeout 10s)..."
consumed=false
if command -v kcat >/dev/null 2>&1; then
  kcat -C -b "$BROKER" -t "$TEST_TOPIC" -o beginning -c 1 -q || true
  consumed=true
fi

if ! $consumed && [ -n "$CONSUMER_CMD" ] && [ -n "$KAFKA_CONTAINER" ]; then
  docker exec -i "$KAFKA_CONTAINER" bash -lc "$CONSUMER_CMD --bootstrap-server localhost:9092 --topic $TEST_TOPIC --from-beginning --timeout-ms 10000" || true
  consumed=true
fi

if ! $consumed; then
  echo "   Cannot consume message: no kcat and no consumer CLI found in container."
  echo "   Use kafka-ui (http://localhost:8080) or install kcat / kafka tools locally to inspect topics."
fi
echo

echo "6) Trigger backend to publish an event (requires backend auth)."
if [ -f ".jwt_token" ]; then
  TOKEN=$(cat .jwt_token)
else
  if [ -x "./scripts/test-auth.sh" ]; then
    echo "   Running scripts/test-auth.sh to obtain token..."
    ./scripts/test-auth.sh "$BACKEND" || echo "   test-auth may have failed; continue manually"
  else
    echo "   test-auth.sh not found or not executable. Please run it to create .jwt_token or set JWT_TOKEN env."
  fi
  TOKEN=$(cat .jwt_token 2>/dev/null || echo "")
fi

if [ -z "$TOKEN" ]; then
  echo "   No JWT token available. Skipping backend publish. Obtain token and re-run."
else
  echo "   Posting event to backend to trigger publish to Kafka..."
  EVENT_PAYLOAD="{\"eventType\":\"IntegrationTest\",\"title\":\"kafka publish test\",\"containerId\":\"TEST123\",\"eventTime\":\"$(date -u +"%Y-%m-%dT%H:%M:%SZ")\"}"
  curl -s -X POST "$BACKEND/api/events" -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d "$EVENT_PAYLOAD" && echo "   Event POSTed"
fi
echo

echo "7) Consume from $CONTAINER_TOPIC topic to confirm backend publish (timeout 15s)..."
if [ -n "$KAFKA_CONTAINER" ] && [ -n "$CONSUMER_CMD" ]; then
  docker exec -i "$KAFKA_CONTAINER" bash -lc "$CONSUMER_CMD --bootstrap-server localhost:9092 --topic ${CONTAINER_TOPIC} --from-beginning --timeout-ms 15000" || echo "   No backend messages received"
elif command -v kcat >/dev/null 2>&1; then
  kcat -C -b "$BROKER" -t "${CONTAINER_TOPIC}" -o beginning -c 5 -q || echo "   No backend messages received"
else
  echo "   Cannot consume backend topic: install kcat or use kafka-ui / kafka container CLI."
fi
echo

echo "8) If you have kafka-ui, open http://localhost:8080 and inspect topics 'port-events' / 'container-events'"
echo ""
echo "Done."
