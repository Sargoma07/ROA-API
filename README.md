# Description

It's a microservices WEB API project for RPG game where there are inventory and shop systems.
Microservices have been developed for this purpose:

- Identity service - authentication with JWT
- Inventory service - collect data and update player inventory, equipment and store of game items
- Payment service - service that handles payment and user account
- Shop service - store price of game items

![[architecture]](./Attachments/architecture.jpg)


# Startup
```bash
docker-compose -f docker-compose-api.yml docker-compose-grafana.yml -p roa-api up -d
```
## Tests  
### Postman 
You can import postman collection in Postman from /tests/postman.

![[integration-tests]](./Attachments/integration-tests.jpg)

There are some test cases into [ROA-API Integrations Tests.postman_collection]: 
- Setup - signup/create a new test user
- Refresh token - process for getting a new access token
- Update inventory - update inventory data for user
- Execute payment - process that handles payment and check correct calculation of payment amount

# Components
- .Net C#
  - App
    - API
      - roa.identity.api
      - roa.inventory.api
      - roa.payment.api
      - roa.shop.api
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