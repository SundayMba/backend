# Genderize API

## Overview

This is a .NET 8 ASP.NET Core Web API that exposes:

- `GET /api/classify?name={name}`

The API calls the Genderize API, extracts `gender`, `probability`, and `count`, renames `count` to `sample_size`, computes `is_confident`, and adds a dynamic `processed_at` timestamp in UTC ISO 8601 format.

## Endpoint

### `GET /api/classify?name={name}`

Success response:

```json
{
  "status": "success",
  "data": {
    "name": "john",
    "gender": "male",
    "probability": 0.99,
    "sample_size": 1234,
    "is_confident": true,
    "processed_at": "2026-04-01T12:00:00Z"
  }
}
```

Error response format:

```json
{
  "status": "error",
  "message": "<error message>"
}
```

## Validation Rules

- `400 Bad Request`: missing or empty `name`
- `422 Unprocessable Entity`: the value cannot be meaningfully processed as a human name
- `404 Not Found`: the upstream API returns `gender = null` or `count = 0`
- `502 Bad Gateway`: upstream API failure
- `500 Internal Server Error`: unexpected server error

Because query parameters arrive as strings in ASP.NET Core, `422` is used for values that do not behave like a real name, for example numeric values, multiple `name` values, boolean-like strings, or malformed content.

## Confidence Logic

`is_confident` is `true` only when:

- `probability >= 0.7`
- `sample_size >= 100`

If either condition fails, the value is `false`.

## CORS

The API allows:

- `Access-Control-Allow-Origin: *`

## Run Locally

```bash
dotnet restore
dotnet build Genderize.sln
dotnet run --project src/Genderize.Api/Genderize.Api.csproj
```

Local HTTP URL:

- `http://localhost:5073`

Swagger is available in development at:

- `http://localhost:5073/swagger`

## Quick Test

```bash
curl "http://localhost:5073/api/classify?name=john"
```

## Run With Docker

Build the image:

```bash
docker build -t genderize-api .
```

Run the container:

```bash
docker run --rm -p 8080:8080 genderize-api
```

Call the API:

```bash
curl "http://localhost:8080/api/classify?name=john"
```

If you want Swagger inside the container too, run:

```bash
docker run --rm -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development genderize-api
```
