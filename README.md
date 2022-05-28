# Interject Dotnet Data Api

### A Dotnet version of the Data Api used to connect Interject to your data sources. This can be deployed within a client's network behind a firewall. Or it can be made public as client needs vary.

---

- ### **[How to setup the API for development](#setup-for-dev)**
- ### **[How to add a new controller](#new-controller)**
- ### **[Working with the request pipeline](#request-pipeline)**

# <a name="setup-for-dev">How to setup the API for development</a>

Steps: _using Visual Studio Code_

1. Clone the repo.
2. You will need dotnet sdk installed.
3. You will also need the dotnet C# extension listed in the .vscode extensions.json file.
4. Navigate to the interject-dotnet-api directory and execute the restore command.
   > dotnet restore
5. You should now be able to now run the application. Press (Ctrl+Shift+D) or use the run and debug menu. <img src="./ReadmeSrc/VSCodeDebug.png">

   - To run Without debugging Pres F5 or enter the command:
     > dotnet run

6. This should use Kestrel serve the application locally at http://localhost:5000
7. You can test by sending a request to the status controlelr http://localhost:5000/api/v1/status

# <a name="new-controller">How to add a new controller</a>

Each controller will likely represent either a connection to a particular type of data source or a logical collection of endpoints for oa series of reports. Each endpoint should follow the basic pipeline flow for handling a request. See working with the request pipeline for more details.

Using the first example there is an SQLController included in this project already. Here is how to create a new controller template. This will create the controller with one endpoint, and three classes.

1. Create a new file in the Controllers directory using the naming convention {Name}Controller.cs.
2. Use code snippets to scaffold the controller. The prefix is 'contr'.
   > contr > TAB

# <a name="request-pipeline">Working with the request pipeline</a>

The InterjectRequestHandler creates a pipeline for processing the InterjectRequest object sent
from the Interject Addin. This class contains a few properties that are intended for use as the
request passes through the phases of a request. The pipeline uses Dependency Injection to consume classes derived from interfaces. It passes itself as a parameter to each of the interface's methods to expose access to the rest of the class. Those phases and interfaces are as follows:

1. Initialize the InterjectRequestHandler
2. Convert the RequestParameters _(IParameterConverter)_
3. Fetch the data _(IDataConnection)_
4. Convert the data _(IResponseConverter)_
5. Return the InterjectResponse object

## 1) **Initialize the InterjectRequestHandler**

Pass the InterjectRequest to the InterjectRequestHandler.Init() method where it will instantiate a new InterjectResponse object. It will store the InterjectRequst, InterjectResponse, and a few other properties described below for use in the rest of the pipeline. The inital InterjectRequset.RequestParameterList is also transferd to the InterjectResponse.RequestParameterList to ensure the Addin at least recieves the original parameters in the response. It is also possible to replace or manipulate those returned parameters during the pipeline if needed.

## 2) **Convert the RequestParameters** _(IParameterConverter)_

The data source you are creating the controller for will likely need the parameters passed in the request to be either converted to another class type or be otherwise processed before they can be used to fetch the data.

The InterjectRequestHandler.ConvertedParameters property is a collection of generic object type intended for storing the processed parameters to be used in the next phase. Since the collection is a list of generic object you can place a collection of any type needed.

## 3) **Fetch the data** _(IDataConnection)_

The Interface requires an implementation of both a sync and async fetch method. However, which you choose to use in the controller's endpoint is up to you.

The data returned is intended to be stored in the InterjectRequestHandler.ReturnData property until the IResponseConverter derived class can transfom it to the collection of ReturnedData in the InterjectRequestHandler.InterjectResponse.

## 4) **Convert the data** _(IResponseConverter)_

Once the data has bee returned from the data source, it needs to be converted into the form the Interject Addin can consume. See the InterjectResponse.ReturnedDataList property for a definition of the required form.

## 5) **Return the InterjectResponse object**

For reverse compatiblilty reasons, the ReturnedData.Data property must be serialized prior to retuning the response to the caller. The InterjectRequestHandler.PackagedResponse property handles this to make it easier for development.
