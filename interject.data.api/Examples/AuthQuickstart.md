# Auth Quickstart (public vs protected + Postman)

This quickstart shows how to verify auth wiring in the Interject .NET Data API.

## Endpoints used
- **Public**
  - `GET /api/v1/status` - health (`true`)
  - `GET|POST /api/v1/status/headers` - echoes method, path, and headers (Authorization is sanitized)
- **Protected**
  - `GET /api/v1/example/protected` - minimal demo; returns a few JWT claims
  - `POST /api/v1/sql` - real pipeline; requires a valid token and inputs

## Config notes
- **Authority**: `https://test-auth.gointerject.com` (matches token `iss`).  
- **Audience** (if enabled): `https://test-auth.gointerject.com/resources` Current code **does not validate audience**.
- Environment files: the launch profile uses `ASPNETCORE_ENVIRONMENT=Development`. Use `appsettings.Development.json` or configure for self.

## Quickstart (Postman)
1. **Base URL**: `http://localhost:5000`
2. **Public health**  
   GET `/api/v1/status` -> **200** body: `true`
3. **Headers echo**  
   GET `/api/v1/status/headers` -> **200**  
   Add `Authorization: Bearer <JWT>` and repeat -> Authorization appears **sanitized**
4. **Protected (claims demo)**  
   GET `/api/v1/example/protected` (no token) -> **401**  
   Add `Authorization: Bearer <JWT>` -> **200** with claims JSON
5. **Protected (SQL)**  
   POST `/api/v1/sql` (no token) -> **401**

Import the collection at:
`interject-dotnet-api/postman/InterjectDotnetApi.postman_collection.json`


## Curl (optional)
```bash
# public
curl http://localhost:5000/api/v1/status
curl -i http://localhost:5000/api/v1/status/headers

# protected (expect 401)
curl -i http://localhost:5000/api/v1/example/protected

# protected with token
curl -i http://localhost:5000/api/v1/example/protected \
  -H "Authorization: Bearer YOUR.JWT.TOKEN"

## Debugging 401s (fast loop)

1. Hit /status/headers with the same headers as your failing request-confirm Authorization is present.
2. Ensure token iss matches configured Authority exactly (watch trailing /).
3. Refresh an expired token.
4. If audience validation is enabled, ensure token aud includes the configured audience.

## See also
- `Examples/ReportFixed.md`, `ReportRange.md`, `ReportVariable.md`, `ReportLookup.md`, `jDropdown.md`, `HandlerPipeline.md`

