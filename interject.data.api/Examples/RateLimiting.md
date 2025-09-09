# Rate Limiting: Gateway and Middleware Options (.NET)
---

## Quick start
- **Gateway (NGINX)** — see Option 1.
- **App middleware (.NET 8 built-in)** — see Option 2.

**Example used throughout:** limit `GET /api/v1/Status/Ping` to **5 requests/min per IP**.

---

## Option 1: API Gateway (NGINX)
*TODO: `limit_req_zone`, `limit_req`, `limit_req_status 429`, plus Traefik/AWS APIG/Azure APIM notes.)*

## Option 2: Application Middleware (.NET 8)
*TODO: `ping`, `[EnableRateLimiting("ping")]` on the Ping action, test commands, and troubleshooting.)*
