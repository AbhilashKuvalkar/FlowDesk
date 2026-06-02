# FlowDesk — Multi-Tenant B2B Support Ticket Platform

A production-grade distributed system demonstrating Clean Architecture,
CQRS, DDD, and microservices patterns on .NET 10 and Azure.

## Architecture

[architecture diagram image here]

## Technology Stack

| Concern                | Technology                          |
|------------------------|-------------------------------------|
| API Framework          | ASP.NET Core 10, Minimal APIs       |
| Architecture Pattern   | Clean Architecture, CQRS, DDD       |
| ORM                    | Entity Framework Core 10            |
| Messaging              | RabbitMQ, MassTransit 8             |
| Caching                | Redis (cache-aside pattern)         |
| Internal Communication | gRPC (Protobuf)                     |
| API Gateway            | GraphQL (Hot Chocolate 13)          |
| Infrastructure         | Azure, AKS, Bicep, Terraform        |
| CI/CD                  | Azure DevOps, GitHub Actions        |
| Observability          | Application Insights, Log Analytics |

## Services

| Service                  | Responsibility                              |
|--------------------------|---------------------------------------------|
| TicketService            | Core domain — tickets, agents, CQRS/DDD    |
| SlaService               | SLA policy management, gRPC server          |
| NotificationService      | Event-driven notifications, RabbitMQ        |

## Key Design Decisions

- **CQRS with MediatR** — commands and queries fully separated,
  pipeline behaviours for cross-cutting concerns
- **Domain Events** — entities raise events, handlers react
  independently — open/closed principle applied
- **Cache-aside pattern** — SLA policies cached in Redis,
  invalidated on update
- **Outbox pattern** — guaranteed message delivery via
  MassTransit transactional outbox
- **Repository + Unit of Work** — infrastructure abstracted
  behind domain interfaces, fully testable

## Running Locally

prerequisites: Docker Desktop, .NET 10 SDK, Azure CLI

```bash
docker-compose up -d        # starts SQL Server, Redis, RabbitMQ
dotnet run --project src/FlowDesk.TicketService
dotnet run --project src/FlowDesk.SlaService
dotnet run --project src/FlowDesk.NotificationService
```

GraphQL playground: http://localhost:5000/graphql
RabbitMQ management: http://localhost:15672 (guest/guest)

## Infrastructure Deployment

```bash
# Bicep
az deployment group create \
  --resource-group flowdesk-dev-rg \
  --template-file infrastructure/bicep/main.bicep \
  --parameters infrastructure/bicep/parameters/dev.bicepparam

# Terraform
cd infrastructure/terraform
terraform init
terraform apply -var-file="terraform.tfvars"
```