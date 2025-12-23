# UrlShortener

A modern URL shortening service built with ASP.NET Core 9.0, featuring user authentication, role-based access control, and a RESTful API.

## Features

- URL shortening with custom codes
- User authentication and authorization
- Role-based access control (Admin/User)
- Short URL management and analytics
- Docker containerization
- Automated testing

## Tech Stack

- **.NET 9.0** - Latest .NET framework
- **ASP.NET Core MVC** - Web framework
- **PostgreSQL** - Database
- **Entity Framework Core** - ORM
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - Input validation
- **JWT Bearer** - Authentication
- **Swagger/OpenAPI** - API documentation
- **Docker** - Containerization
- **xUnit** - Unit testing

## Prerequisites

- .NET 9.0 SDK
- Docker Engine 20.10+
- Docker Compose 2.0+
- PostgreSQL 16+ (or use Docker)

## Quick Start

### Using Docker Compose (Recommended)

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd UrlShortener
   ```

2. **Create environment file**
   ```bash
   nano .env
   ```
   
   Add the following variables (or use defaults):
   ```env
   POSTGRES_USER=postgres
   POSTGRES_PASSWORD=your_secure_password
   POSTGRES_DB=urlshortener
   POSTGRES_PORT=5432
   WEB_PORT=8080
   ADMIN_EMAIL=admin@example.com
   ADMIN_PASSWORD=ChangeMe123!
   ADMIN_FIRST_NAME=Admin
   ADMIN_LAST_NAME=User
   ```

3. **Deploy locally**
   
   Everything runs in one command - database and application together:
   ```bash
   docker-compose up -d --build
   ```

4. **Access the application**
   - Web UI: `http://localhost:8080`
   - Swagger UI: `http://localhost:8080/swagger`
   - Health Check: `http://localhost:8080/health`

## Project Structure

```
UrlShortener/
├── UrlShortener.Web/          # ASP.NET Core MVC web application
├── UrlShortener.BLL/          # Business logic layer
├── UrlShortener.DAL/          # Data access layer
│   ├── Commands/              # CQRS commands
│   ├── Queries/               # CQRS queries
│   ├── Database/              # DbContext and entities
│   └── Migrations/            # EF Core migrations
├── UrlShortener.Shared/       # Shared models and utilities
├── UrlShortener.Tests/        # Unit tests
├── compose.yaml               # Docker Compose configuration
└── .github/workflows/         # CI
```

## Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `POSTGRES_USER` | PostgreSQL username | `postgres` |
| `POSTGRES_PASSWORD` | PostgreSQL password | `postgres` |
| `POSTGRES_DB` | Database name | `urlshortener` |
| `POSTGRES_PORT` | PostgreSQL port | `5432` |
| `WEB_PORT` | Web application port | `8080` |
| `ADMIN_EMAIL` | Default admin user email | `admin@example.com` |
| `ADMIN_PASSWORD` | Default admin user password | `ChangeMe123!` |
| `ADMIN_FIRST_NAME` | Default admin first name | `Admin` |
| `ADMIN_LAST_NAME` | Default admin last name | `User` |

## API Documentation

Once the application is running, access the Swagger UI at:
- **Swagger UI**: `http://localhost:8080/swagger`

The API provides endpoints for:
- User authentication (login, register)
- Short URL creation and management
- URL redirection
- Admin operations

## Testing

Run tests using:
```bash
dotnet test
```

Tests include:
- Unit tests for business logic
- Command handler tests
- Service tests

## Monitoring

### Health Checks
```bash
# Check application health
curl http://localhost:8080/health

# Check container status
docker-compose ps

# View container logs
docker-compose logs -f web
```


