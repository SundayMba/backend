# Backend Wizards Stage 2 API

## Overview

This project is a .NET 8 ASP.NET Core Web API backed by PostgreSQL.

It supports:

- profile creation from external APIs
- persistent profile storage
- advanced filtering
- sorting
- pagination
- rule-based natural language search
- idempotent seeding of 2026 profiles

The Stage 2 query engine is built on a PostgreSQL `profiles` table with these persisted fields:

- `id`
- `name`
- `gender`
- `gender_probability`
- `age`
- `age_group`
- `country_id`
- `country_name`
- `country_probability`
- `created_at`

## External APIs

- `Genderize` -> gender and probability
- `Agify` -> age
- `Nationalize` -> top predicted country

## Seed Data

The repository includes `data/seed_profiles.json`.

- The file contains `2026` profiles
- Startup seeding is enabled by default
- Re-running the application does not create duplicate seeded records

The seed path is configured through:

```json
"SeedData": {
  "Enabled": true,
  "FilePath": "data/seed_profiles.json"
}
```

## Endpoints

### `POST /api/profiles`

Creates a profile from the external APIs. If the name already exists, the existing record is returned with:

```json
{
  "status": "success",
  "message": "Profile already exists",
  "data": { }
}
```

### `GET /api/profiles/{id}`

Returns a single stored profile.

### `DELETE /api/profiles/{id}`

Deletes a stored profile and returns `204 No Content`.

### `GET /api/profiles`

Supports combined filters, sorting, and pagination.

Supported filters:

- `gender`
- `age_group`
- `country_id`
- `min_age`
- `max_age`
- `min_gender_probability`
- `min_country_probability`

Sorting:

- `sort_by=age|created_at|gender_probability`
- `order=asc|desc`

Pagination:

- `page` default `1`
- `limit` default `10`
- `limit` max `50`

Response shape:

```json
{
  "status": "success",
  "page": 1,
  "limit": 10,
  "total": 2026,
  "data": []
}
```

### `GET /api/profiles/search`

Rule-based natural language search.

Query parameter:

- `q`

Supported examples:

- `young males`
- `females above 30`
- `people from angola`
- `adult males from kenya`
- `male and female teenagers above 17`

Rules implemented:

- `young` => `min_age=16` and `max_age=24`
- `male and female` means no gender filter is applied
- country names are mapped to ISO codes using a rule-based lookup
- queries that cannot be interpreted return:

```json
{
  "status": "error",
  "message": "Unable to interpret query"
}
```

## Error Handling

All errors use:

```json
{
  "status": "error",
  "message": "<error message>"
}
```

Status behavior:

- `400` for missing or empty parameters
- `422` for invalid parameter types
- `404` for missing profiles
- `502` for invalid upstream API responses
- `500` for unexpected server failures

Query validation failures return:

```json
{
  "status": "error",
  "message": "Invalid query parameters"
}
```

Invalid upstream responses return:

```json
{
  "status": "error",
  "message": "Genderize returned an invalid response"
}
```

or:

```json
{
  "status": "error",
  "message": "Agify returned an invalid response"
}
```

or:

```json
{
  "status": "error",
  "message": "Nationalize returned an invalid response"
}
```

## CORS

The API allows:

- `Access-Control-Allow-Origin: *`

## Local Run

### PostgreSQL already running locally

Default connection string:

```text
Host=localhost;Port=5432;Database=genderize_stage2_db;Username=postgres;Password=postgres
```

Run:

```bash
dotnet restore Genderize.sln
dotnet build Genderize.sln
dotnet run --project src/Genderize.Api/Genderize.Api.csproj
```

Development URL:

- `http://localhost:5073`

Swagger:

- `http://localhost:5073/swagger`

### Run API and PostgreSQL with Docker Compose

```bash
docker compose up --build
```

That starts:

- PostgreSQL on `localhost:5432`
- API on `http://localhost:8080`

Swagger:

- `http://localhost:8080/swagger`

## Example Requests

Create a profile:

```bash
curl -X POST "http://localhost:5073/api/profiles" \
  -H "Content-Type: application/json" \
  -d '{"name":"ella"}'
```

Filter and paginate:

```bash
curl "http://localhost:5073/api/profiles?gender=male&country_id=NG&min_age=25&page=1&limit=10"
```

Sort by age descending:

```bash
curl "http://localhost:5073/api/profiles?sort_by=age&order=desc"
```

Natural language search:

```bash
curl "http://localhost:5073/api/profiles/search?q=young%20males%20from%20nigeria&page=1&limit=10"
```

## Implementation Notes

- PostgreSQL persistence uses EF Core with Npgsql
- The schema is created automatically on startup with `EnsureCreated`
- Seed import runs at startup after schema creation
- Filters are combined with logical `AND`
- Sorting and pagination are executed at the database-query level
- The Stage 0 `GET /api/classify` endpoint is still available
