# Structured Logging (Serilog)

## TL;DR
- Disabled by default. Enable by setting `"InterjectLogging": { "UseBuiltIn": true }` in `appsettings.{Environment}.json`.
- Logs write to `.\Logs\app-YYYYMMDD.json` (rolling daily, 30 files by default).
- Request summaries: uncomment `app.UseSerilogRequestLogging();` in `Startup.cs`.

## Enable
1. In `appsettings.Development.json`:
   ```json
   "InterjectLogging": { "UseBuiltIn": true }
2. (Optional) Uncomment in `Startup.cs`:
    ```csharp
    // app.UseSerilogRequestLogging();
    // app.UseMiddleware<CorrelationIdMiddleware>();
    // app.UseMiddleware<AuthFailureTelemetryMiddleware>();
    ```
## Where logs go
- Local file sink:`.\Logs\app-*.json` (NDJSON).
- Add a shipper (e.g., Filebeat/Fluent Bit) later to send to your SIEM/ELK.

## Tuning Noise and Verbosity
```json
"Serilog": {
  "MinimumLevel": {
    "Default": "Information",
    "Override": {
      "Microsoft": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "System": "Warning"
    }
  }
}
```
