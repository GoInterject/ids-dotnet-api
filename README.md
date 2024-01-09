# Interject .Net API

Interject is an Addin for the Microsoft Excel spreadsheet that connects users to their data. Through the Interject Portal, users can set up Data Portals to connect with a Data Connection and pull data directly into the Excel report.

This Interject .Net API allows data flow from custom sources through a web API.

The Interject .Net API consists of 2 packages: Interject.api and Interject.data.api.

## Interject.api 

Made public on the NuGet package manager. This package contains the core Interject classes and models to format data and handle data flow to and from the Interject Addin. This package can be imported directly into your custom api:

```csharp
using Interject.Api;
```

## Interject.data.api

This package example controllers that use the Interject.api to send data from a custom source to the Interject Addin.

For more information, consult the [readme files](https://github.com/GoInterject/ids-dotnet-api/tree/main/interject.data.api/Examples).

## Links

- [Interject Addin Documentation](https://docs.gointerject.com/)
- [Interject Portal Site](https://portal.gointerject.com/login.html)
- [Interject .Net API Host Setup Documentation](https://docs.gointerject.com/wApi/dot-net-api-setup.html)
- [Interject .Net API Developing Setup Documentation](https://github.com/GoInterject/ids-dotnet-api/blob/main/interject.data.api/README.md)
- [Interject .Net API Repository](https://github.com/GoInterject/ids-dotnet-api)
- [Interject.api NuGet Package](https://www.nuget.org/packages/Interject.07.14.23/)
