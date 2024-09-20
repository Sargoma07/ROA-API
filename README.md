# Description

It's a WEB API project for ROA game.

# Startup
```bash
docker-compose -f docker-compose-api.yml docker-compose-grafana.yml -p roa-api up -d
```

# Components
- API
  - roa.identity.api - identity service
  - roa.inventory.api - inventory service
- DB 
  - Mongo  
- Grafana
  - Loki - logging tool
  - Alloy - collector for logs to loki 
  - Tempo - tracing tool
  - Prometheus - monitoring tool
- Kafka - event bus, message broker 

## Kafka
Messages into kafka serialize to protobuf format.  
### Protobuf
It's a binary protocol buffer. 
Command to generate class from proto:
```bash
protoc  --csharp_out=./ROA.Identity.API/Domain/Events ./Protos/UserCreatedEvent.proto
```