# Instructions for GitHub Copilot

## Context about the repository

These projects are part of an API solution designed using .NET Core and ASP.NET Core. The primary functionalities include API integration, database interaction, JWT-based authentication, and Swagger-based API documentation.

### Technology Stack

- **Framework**: .NET Core/ASP.NET Core
- **Authentication**: JWT Bearer Tokens
- **Database**: SQL Server (via Microsoft.Data.SqlClient)
- **Utilities**: Configuration management and HTTP extensions
- **API Documentation**: Swagger (via Swashbuckle.AspNetCore)

## Dependencies, Packages, and Tools Used

1. **API Integration**:

   - `Interject.Api` (1.1.6)

2. **Authentication**:

   - `Microsoft.AspNetCore.Authentication.JwtBearer` (6.0.5)
     - Provides JWT Bearer Token authentication middleware.

3. **Utilities**:

   - `Microsoft.AspNetCore.Http.Extensions` (2.2.0)
     - Adds utility methods for working with HTTP requests.

4. **Database**:

   - `Microsoft.Data.SqlClient` (4.1)
     - SQL Server data provider for ADO.NET.

5. **Configuration Management**:

   - `Microsoft.Extensions.Configuration` (6.0.0)
     - Provides access to configuration data in .NET applications.
   - `Microsoft.Extensions.Configuration.Abstractions` (6.0.0)
     - Defines abstractions for configuration settings.

6. **API Documentation**:

   - `Swashbuckle.AspNetCore` (6.0.0)
     - Enables generating Swagger/OpenAPI documentation for ASP.NET Core APIs.

7. **Framework**:

   - .NET Core / ASP.NET Core (version 6.0)
     - The core framework for building modern, scalable applications.

8. **Swagger/OpenAPI**:

   - Used for API documentation and client generation via Swashbuckle.

9. **SQL Server**:

   - Database interactions facilitated through Microsoft.Data.SqlClient.

10. **Authentication**:

- JWT-based authentication via Microsoft.AspNetCore.Authentication.JwtBearer.

### Code Style and Standards

- Use PascalCase for public methods and properties, camelCase for variables.
- Follow .NET asynchronous programming practices (`async`/`await`).
- Utilize dependency injection for services.
- Prioritize testable code with proper abstraction.
- Generate comprehensive Swagger documentation for APIs.

## What You Want Copilot To Do

### Focus Areas

- Suggest secure and scalable code for JWT authentication.
- Assist with SQL query optimizations using `Microsoft.Data.SqlClient`.
- Provide code snippets for Swagger configuration and documentation.
- Generate boilerplate code for DI and middleware configurations.

### Exclusions

- Avoid suggesting unrelated libraries unless justified.
- Stick to .NET 6.0-compatible packages and features.

### Preferred Practices

- Utilize `Microsoft.Extensions.Configuration` for configuration management.
- Write clear comments for API endpoints and middleware.
- Generate comprehensive unit tests for API controllers and services.
