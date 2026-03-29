# CommunicationService

Production notification microservice (.NET 8, Clean Architecture).

## Database

1. Apply `../../06_Communication.sql` **after** `01_SharedDomain.sql` (same database as `ReferenceDataValue` FKs).
2. Seed `ReferenceDataValue` rows for channel types, notification/queue/delivery statuses, and insert `COM_NotificationTemplate` rows per facility.
3. Align `appsettings.json` → `Communication:ReferenceValueIds` with the **actual** `ReferenceDataValue.Id` values for your tenant (placeholders `1–42` are examples only).

## Configuration

- `ConnectionStrings:CommunicationDatabase` — must point to the database that contains `COM_*` tables and `ReferenceDataValue`.
- `Communication:Smtp`, `Sms`, `WhatsApp` — provider settings (environment variables override in Docker).
- `Jwt:*` — same pattern as other TriVita microservices.

## REST API

- `POST /api/v1/notifications`
- `POST /api/v1/notifications/send-template`
- `GET /api/v1/notifications/{id}`
- `GET /api/v1/notifications/logs`
- `GET /api/v1/notifications/templates`

Swagger: `/swagger` (Development). JWT + XML comments.

## Docker

From repository root (`HealthcarePlatform` parent containing `BuildingBlocks` and `CommunicationService`):

```bash
docker build -f CommunicationService/Dockerfile -t trivita/communication-service .
```

Set `ASPNETCORE_ENVIRONMENT`, `ConnectionStrings__CommunicationDatabase`, `Jwt__Key`, and `Communication__*` via environment variables.

## Other services

HMS, LIS, LMS, and Pharmacy reference `CommunicationService.Contracts` and call `INotificationApiClient` via `*NotificationHelper` — **no** direct SMTP/SMS from domain modules.
