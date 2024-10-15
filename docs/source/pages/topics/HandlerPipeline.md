---
Title: Handler Pipeline
---

# Working with the Interject Request Handler Pipeline

The `Requesthandler` class is used as a pipeline to connect with a data in a flexible workflow. Data can be fetched, converted, and transferred back to Interject via this class.

## Working With the Pipeline

The `RequestHandler` creates a pipeline for processing the `InterjectRequest` object sent from the Interject Addin. This class contains a few properties that are intended for use as the request passes through the phases of the request pipeline. The pipeline uses Dependency Injection to consume classes derived from interfaces. It passes itself as a parameter to each of the interface's methods to expose access to the rest of the class. Those phases and interfaces are as follows:

1. Initialize the `RequestHandler`
2. Convert the `RequestParameters` _(IParameterConverter)_
3. Fetch the data _(IDataConnection)_
4. Convert the data _(IResponseConverter)_
5. Return the `InterjectResponse` object

## Connection Strings

For the purpose of the SqlController example, this API uses the `appsettings.json` to hold connections strings. On startup, the program will initialize these strings in Configurations. The connection strings are available via dependency injection by requiring a `ConnectionStringOptions` object in the controller's constructor. The `InterjectRequest.PassThroughCommand` will contain the ConnectionStringName. The connection is established by initializing the `SqlDataConnectionAsync` object with the connection strings.

## Sql Controller

The `SqlController` class is an example using the `RequestHandler` pipeline. The following outlines the pipeline flow used in this example:

1. Inits the `RequestHandler`
2. Inits the Handler.`ParameterConverter`
3. Inits the Handler.`DataConnectionAsync`
4. Inits the Handler.`ResponseConverter`
5. Returns the Response by calling Handler.`ReturnResponseAsync`

There are 4 internal classes in this example:

- `SqlParameterConverter` : Used to convert the data types of the parameters sent by Interject from the Excel Report to the API. Data types must be converted first to be used to fetch data.

- `SqlDataConnectionAsync` : Used to establish a connection with the data source. Establishes the connection string, parameters, and fetches data.

- `ParamPair` : Class for holding a pair of parameters, one from the Interject request and its converted SQL parameter.

- `SqlResponseConverter` : Class for converting the data fetched from the data source to a format Interject can consume.

## How to Add A New Controller

Each controller will likely represent either a connection to a particular type of data source or a logical collection of endpoints for a series of reports. Each endpoint should follow the basic pipeline flow for handling a request. See [Working with the request pipeline](#working-with-the-pipeline) for more details.

Using the first example there is an SQLController included in this project already. Here is how to create a new controller template. This will create the controller with one endpoint, and four classes; one for each of the interfaces. Note that the `IDataConnectionAsync` and `IDataConnection` are interchangeable depending on your needs and only one can be used per endpoint.

1. Create a new file in the Controllers directory using the naming convention {Name}Controller.cs.
2. Use code snippets in VS Code to scaffold the controller. The prefix is 'c-pipe'.
   > c-pipe > TAB
3. Type the {Name} of the controller as prompted by the snippet and press TAB
4. You can now begin to customize your Pipeline interface implementations.

### 1) **Instantiate the InterjectRequestHandler**

Create an instance of the **InterjectRequestHandler** passing the InterjectRequest into the constructor. This will instantiate a new `InterjectResponse` object and store the `InterjectRequest`. The constructor will also instantiate a couple other data storage properties described below for use in the rest of the pipeline. The initial `InterjectRequest.RequestParameterList` is also transferred to the `InterjectResponse.RequestParameterList` to ensure the Interject add-in at least receives the original parameters in the response. It is also possible to replace or manipulate those returned parameters during the pipeline if needed.

### 2) **Convert the RequestParameters** _(IParameterConverter)_

The data source you are creating the controller for will likely need the parameters passed in the request to be either converted to another class type or to be otherwise processed before they can be used to fetch the data.

The `InterjectRequestHandler.ConvertedParameters` property is a collection of generic object type intended for storing the processed parameters to be used in the next phase. Since the collection is a list of generic object you can place a collection of any type needed.

### 3) **Fetch the data** _(IDataConnection)_ OR _(IDataConnectionAsync)_

The Interface can use an implementation of either a sync or async fetch method. However, only one implementation can be used per endpoint. Note that the `InterjectRequestHandler.ReturnResponse()` and `InterjectRequestHandler.ReturnResponseAsync()` use those implementations respectively.

The data returned is intended to be stored in the `InterjectRequestHandler.ReturnData` property until the `IResponseConverter` derived class can transform it to the collection of `ReturnedData` in the `InterjectRequestHandler.InterjectResponse`.

### 4) **Convert the data** _(IResponseConverter)_

Once the data has been returned from the data source, it needs to be converted into the form the Interject Addin can consume. See the `InterjectResponse.ReturnedDataList` property for a definition of the required form.

### 5) **Return the InterjectResponse object**

Call the `InterjectRequestHandler.ReturnResponse()` or `InterjectRequestHandler.ReturnResponseAsync()` method depending on your implementation to complete the pipeline and return the `InterjectResponseDTO`

Note: For reverse compatibility reasons, the `ReturnedData.Data` property must be serialized prior to retuning the response to the caller. The `InterjectRequestHandler.PackagedResponse` property handles this to make it easier for development.
