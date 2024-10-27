# Description

It's a WEB API project for ROA game.

![[architecture]](./Attachments/architecture.jpg)


# Startup
```bash
docker-compose -f docker-compose-api.yml docker-compose-grafana.yml -p roa-api up -d
```
## Tests  
### Postman 
You can import postman collection in Postman from /tests/postman.

# Components
- .Net C#
  - App
    - API
      - roa.identity.api - identity service
      - roa.inventory.api - inventory service
      - roa.payment.api - service that handles payment
      - roa.shop.api - shop service
    - Utilities
      - roa.data.migrations - database migrations
  - Libs
    - Serilog - logging
    - OpenTelemetry - tracing
    - MongoDB.Driver - database
    - Confluent.Kafka - event bus
    - Google.Protobuf - to serialize/deserialize binary format
    - AutoMapper - mapping
    - Refit - http client for REST api
    - Polly - to provide resilience strategies such as Retry, Circuit Breaker, Hedging, Timeout, Rate Limiter and Fallback
- DB 
  - Mongo  
  - Redis
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

## Grafana
Tracing and logs related together with traceId. You can see result of collected data in [Grafana](http://localhost:3000).

### Tempo
![[tracing]](./Attachments/tracing.jpg)

### Loki
![[logs]](./Attachments/logs.jpg)