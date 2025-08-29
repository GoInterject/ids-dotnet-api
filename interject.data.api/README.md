<p align="center">
    <img src="./banner.png" />
    <br />
    <br />
    <i>
    ⚒️ A Modular Toolkit for Creating Interject Data API's in .Net ⚒️
    </i>
    <br />
    <br />
    <img
        src="https://img.shields.io/badge/v1.0.0-white?color=black&style=for-the-badge&logo=git"
        alt="Current Version"
    />
    <img
        src="https://img.shields.io/badge/LICENSE-Apache 2.0-black?color=black&style=for-the-badge"
        alt="LICENSE"
    />
</p>

<br>

# Interject Data API (.Net Edition)

The Interject Data API for .Net. This API fetches data from custom sources allowing Interject to integrate data from more locations.

<br>
<br>

# Table of Contents

- [How to Set Up the API for Development](#how-to-setup-the-api-for-development)
- [Configuring Auth](configure-auth)
- [Status Controller](#status-controller)
- [SQL Controller](#sql-controller)
- [How to Add a New Controller](#how-to-add-a-new-controller)
- [Settings & Configurations](#settings-configurations)

<br>
<br>

<h1 id="how-to-setup-the-api-for-development">How to Set Up the API for Development</h1>

Steps: _using Visual Studio Code_

1. Clone the [repository](https://github.com/GoInterject/ids-dotnet-api) from Github.
2. You will need to install the Framework [.Net 7](https://dotnet.microsoft.com/en-us/download/dotnet) or greater.
3. You will need the [.Net SDK](https://dotnet.microsoft.com/download) for developing.
4. You will also need the [C# Dev Kit](https://code.visualstudio.com/docs/languages/csharp) extension for VSCode.
5. Navigate to the interject-dotnet-api directory and execute the restore command.

```csharp
dotnet restore
```

5. You should now be able to run the application. Press (Ctrl+Shift+D) or use the run and debug menu.

   <img src="./ReadmeSrc/VSCodeDebug.png">

   > Note: To run without debugging press F5 or enter the command:

```csharp
dotnet run
```

6. This API uses Kestrel to serve the application locally at the default base URL of http://localhost:5000. You can configure the starting port by modifying the file: `interject.data.api\interject-dotnet-api\Properties\launchSettings.json`

7. You can test by sending a request to the status controller http://localhost:5000/api/v1/status

<br>

<h1 id="configuring-auth">Configuring Auth</h1>

Out of the box, Interject.Data.Api supports OIDC. It is preconfigured to use Interject's identity provider. However, you can configure it to use another OIDC compliant identity provider.

If you are using federated logins to access Interject, your provider's access token will be sent with the request. Update the appsettings.json

```JSON
{
   "Authority": "<your identity providers url>"
}
```
See **[Examples/AuthQuickstart.md](./Examples/AuthQuickstart.md)** for public vs protected examples and a Postman recipe.

<h1 id="status-controller">Status Controller</h1>

As an additional tool for testing and to assist in future troubleshooting of client APIs, a status controller is available. There are two endpoints:

- {base url}/api/v1/status

  - Returns "true"

- {base url}/api/v1/status/options
  - Returns information stored in the class `ApplicationOptions` as configured in "Applications" section of the `appsettings.json`.

<h1 id="sql-controller">SQL Controller</h1>

The SQL Controller is meant as a single endpoint to pass request parameters to a stored procedure. All the RequestParameter objects in the InterjectRequest object are converted to SqlParameters then are sent to the Stored Procedure named in the Data Portal.

<h1 id="how-to-add-a-new-controller">How to Add a New Controller</h1>

Each controller will likely represent either a connection to a particular type of data source or a logical collection of endpoints for a series of reports.

1. Initialize the `InterjectResponse` & `IdsTable`
2. (Optional) Get the Request parameters
3. Process the Data
4. Build the Columns & Rows
5. Add the `IdsTable` to the Response

## 1) Initialize the Interject_ResponseDTO & Interject_Table

The `InterjectResponse` is initialized using the `InterjectRequest` so that all the Request Parameters can be copied to the Response object.

The `IdsTable` is initialized as well. You may initialize it with a table name if you wish. This object will hold all the columns and rows you will add in following steps.

## 2) (Optional) Get the Request Parameters

If your Interject Request contains parameters, you can get them and store them as local objects in order to use them in your logic for fetching the data. If your Interject Request is including data to save to your data source, you will need to get this data via the `Interject_XMLDataToSave` parameter.

To see a complete guide to Request Parameters, see the readme `portal_parameters` in the interject.data.api\Examples folder.

## 3) Process the Data

Here is where you can add your logic to process the incoming data from the Request. The `InterjectRequest` object should have everything you need to do this. You will need to fetch the data from your own source in order to store them into the Interject Table. If you are performing a save, you will need to use the data from the report sent to the API via the `Interject_XMLDataToSave` parameter.

## 4) Build the Columns & Rows

Each column in the return data needs to be built and added to the `IdsTable`. Each property is set by default but you can override any property to customize your table.

Likewise, each row of data needs to be added to the table. Interject expects the &lt;string&gt; data type so each row is simply a List of &lt;string&gt;.

## 5) Add the IdsTable To the Response

When all the data is processed and added to the InterjectTable, simply add the table to the Response using te `AddReturnedObject` method. This will serialize the table in order for Interject to consume it.

All that is left is to return the Response.

<br>

<h1 id="settings-configurations"> Settings & Configurations </h1>

This API uses a number of classes and json files to configure and apply settings:

- `ApplicationOptions` : Class that contains the name of the data api which will let us know language and framework information. It also contains the version of the API installed to make it easier during support calls. The version correlates directly to the tag on the main branch of the repo.

- `launchSettings.json` : Contains settings relating to launching the API, including web server settings and Swagger.

- `appsettings.json` : Contains configurations for the app such as the name and version. Also contains connections strings to connect to a data source.
