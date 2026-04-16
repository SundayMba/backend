# Backend Wizards Stage 1 API

## Overview

This project is a .NET 8 ASP.NET Core Web API backed by PostgreSQL.

It accepts a name, calls three external APIs, applies classification logic, stores the resulting profile, prevents duplicate records for the same name, and exposes endpoints to create, read, filter, and delete stored profiles.

External APIs used:

- `Genderize` for gender and probability
- `Agify` for age
- `Nationalize` for country prediction

## Implemented Endpoints

### `POST /api/profiles`

Request body:

```json
{
  "name": "ella"
}
```

Creates a new profile or returns the existing one when the same normalized name already exists.

### `GET /api/profiles/{id}`

Returns a single stored profile by UUID.

### `GET /api/profiles`

Optional query parameters:

- `gender`
- `country_id`
- `age_group`

Filtering is case-insensitive.

### `DELETE /api/profiles/{id}`

Deletes a stored profile and returns `204 No Content`.

### `GET /api/classify`

The Stage 0 endpoint is still available.

## Classification Rules

- Age group:
  - `0-12` => `child`
  - `13-19` => `teenager`
  - `20-59` => `adult`
  - `60+` => `senior`
- Nationality:
  - pick the country with the highest probability from `Nationalize`

## Persistence Rules

- PostgreSQL is used for storage
- Duplicate detection is based on normalized name
- Names are trimmed and compared case-insensitively
- All IDs are generated as UUID v7
- All timestamps are UTC ISO 8601

## Error Handling

All errors use:

```json
{
  "status": "error",
  "message": "<error message>"
}
```

Status codes:

- `400` for missing or empty name
- `422` for invalid type or invalid string-like input
- `404` for missing profile
- `502` for invalid upstream API responses
- `500` for unexpected server failures

Upstream invalid-response messages:

- `Genderize returned an invalid response`
- `Agify returned an invalid response`
- `Nationalize returned an invalid response`

## CORS

The API allows:

- `Access-Control-Allow-Origin: *`

## Local Run With PostgreSQL

### Option 1: run PostgreSQL yourself

Default connection string in development:

```text
Host=localhost;Port=5432;Database=genderize_db;Username=postgres;Password=postgres
```

Run:

```bash
dotnet restore Genderize.sln
dotnet build Genderize.sln
dotnet run --project src/Genderize.Api/Genderize.Api.csproj
```

Development URL:

- `http://localhost:5073`

Swagger in development:

- `http://localhost:5073/swagger`

### Option 2: run everything with Docker Compose

```bash
docker compose up --build
```

That starts:

- PostgreSQL on `localhost:5432`
- API on `http://localhost:8080`

Swagger:

- `http://localhost:8080/swagger`

## Quick Test Commands

Create profile:

```bash
curl -X POST "http://localhost:5073/api/profiles" \
  -H "Content-Type: application/json" \
  -d '{"name":"ella"}'
```

List profiles:

```bash
curl "http://localhost:5073/api/profiles"
```

Filter profiles:

```bash
curl "http://localhost:5073/api/profiles?gender=male&country_id=NG&age_group=adult"
```

## Docker

Build the API image:

```bash
docker build -t genderize-api .
```

Run the API image directly:

```bash
docker run --rm -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5432;Database=genderize_db;Username=postgres;Password=postgres" \
  genderize-api
```

## Notes

- The database schema is created automatically at startup via EF Core `EnsureCreated`
- The service does not store upstream-invalid profiles
- `GET /api/profiles` intentionally returns a reduced list shape to match the assessment contract
