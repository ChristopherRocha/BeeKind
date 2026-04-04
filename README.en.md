# BeeKind

BeeKind is an ASP.NET Core API in active development for managing users, contacts, and events, with JWT-based authentication, Identity, and PostgreSQL persistence through Entity Framework Core.

This repository is evolving continuously. The structure, API contracts, business rules, and implementation details may change over time as the project grows.

## Overview

The project is organized into layers to separate responsibilities and keep the code easier to maintain:

- **Presentation**: API controllers, middlewares, and helpers.
- **Application**: services and DTOs used by the application.
- **Domain**: business entities.
- **Infrastructure**: data access, repositories, and database integration.

Today the API covers the following main flows:

- authentication and password management;
- CRUD for users;
- CRUD for contacts;
- CRUD for events linked to contacts;
- password recovery through token.

## Technologies Used

- .NET 10 / ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- ASP.NET Core Identity
- JWT Bearer Authentication
- Swagger / OpenAPI
- Swashbuckle
- Built-in ASP.NET Core dependency injection

## Applied Best Practices

- Layered architecture to reduce coupling.
- DTOs for API input and output.
- Repositories and services to isolate business rules and persistence.
- JWT authentication for protected endpoints.
- Identity for credential and token management.
- Global exception handling middleware.
- Explicit mapping between the application user and `IdentityUserId` to avoid identifier drift.
- Automatic creation of the `AboveAll` and `User` roles at startup.
- Swagger enabled in development with Bearer token support.
- Versioned database migrations.

## Project Structure

```text
src/
  Application/
    DTOs/
    Services/
  Domain/
    Entities/
    Exceptions/
  Infrastructure/
    Data/
    Repositories/
  Presentation/
    Controllers/
    Helpers/
    Middlewares/
Migrations/
Properties/
Program.cs
```

## Prerequisites

- .NET SDK 10
- PostgreSQL
- Access to a configured database, locally or in the cloud
- A valid email configuration if password recovery is used

## Configuration

Before running, configure the local environment or environment variables with:

- `ConnectionStrings:DefaultConnection`
- `Jwt:Key`
- `Jwt:Issuer`
- `Jwt:Audience`
- `EmailSettings:FromEmail`
- `EmailSettings:SmtpHost`
- `EmailSettings:SmtpPort`
- `EmailSettings:SmtpUser`
- `EmailSettings:SmtpPass`

Important note: real secrets should not be committed. The recommended approach is to use User Secrets, environment variables, or another secure secret store.

## How to Run

1. Restore dependencies:

```bash
dotnet restore
```

2. Apply migrations for both contexts if needed:

```bash
dotnet ef database update --context AppDbContext
dotnet ef database update --context IdentityEfDbContext
```

3. Start the application:

```bash
dotnet run --project BeeKind.csproj
```

In development, the API is typically available at:

- `http://localhost:5444`
- `https://localhost:7049`

Swagger is available only in development.

## Authentication

Protected endpoints use a Bearer token. After logging in, copy the returned token and use the Authorize button in Swagger or send the header:

```http
Authorization: Bearer YOUR_TOKEN_HERE
```

## Main Endpoints

### Authentication

- `POST /api/Auth/register`
- `POST /api/Auth/login`
- `POST /api/Auth/change-password`
- `POST /api/Auth/reset-password`
- `POST /api/Auth/forgot-password`
- `DELETE /api/Auth/delete-user`

### Users

- `GET /api/User`
- `GET /api/User/{id}`
- `POST /api/User`
- `PUT /api/User/{id}`
- `DELETE /api/User/{id}`

### Contacts

- `GET /api/Contact`
- `GET /api/Contact/{id}`
- `POST /api/Contact`
- `PUT /api/Contact/{id}`
- `DELETE /api/Contact/{id}`

### Events

- `GET /api/Event`
- `GET /api/Event/{id}`
- `GET /api/Event/contact/{contactId}`
- `POST /api/Event`
- `PUT /api/Event/{id}`
- `DELETE /api/Event/{id}`

## Usage Examples

### Register

```json
{
  "name": "Maria Silva",
  "email": "maria@example.com",
  "password": "StrongPassword123!",
  "phoneNumber": "11999999999",
  "confirmPassword": "StrongPassword123!"
}
```

### Login

```json
{
  "email": "maria@example.com",
  "password": "StrongPassword123!"
}
```

### Create Contact

```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "phoneNumber": "11988887777"
}
```

### Create Event

```json
{
  "contactId": 1,
  "title": "Follow-up meeting",
  "date": "2026-04-04T10:00:00Z",
  "location": "Online",
  "message": "Remember to send the materials before the meeting",
  "description": "Event linked to a specific contact"
}
```

## Architectural Notes

- The application uses two database contexts: one for the main domain and one for Identity.
- The relationship between user, contacts, and events is handled with explicit keys and navigations.
- The API reads the authenticated user's identifier from claims to restrict resource operations to the owner.
- Error handling is centralized to standardize responses.

## Current Status

This project is actively being developed. Rule changes, structural updates, endpoint contract changes, and new features may be added or adjusted over time.

## License

Not defined yet.