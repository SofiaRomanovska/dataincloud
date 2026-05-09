# Product Store API

CRUD REST API for a product store resource built with layered architecture, DDD-oriented boundaries, EF Core, SQL Server, MongoDB, pagination, soft delete, unit tests, and integration tests.

## Getting Started

1. Checkout the feature branch `feat/part_2`.
2. Ensure you have .NET 9 installed.
3. Use Docker Compose to spin up the databases and API.

```bash
docker-compose up -d --build
```

## Resources

The API exposes products at `/api/products` and authors at `/api/authors`.
