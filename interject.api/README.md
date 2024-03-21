# About

Interject.Api provides the classes used by Interject. Use this api to extract data coming from Interject and to package responses.

# Basic Usage

Use the InterjectRequest as the only parameter in the body of your POST endpoints.

``` CS
[HttpPost("ReportSave")]
[ProducesResponseType(typeof(InterjectResponse), 200)]
public InterjectResponse TestReportSave([FromBody] InterjectRequest interjectRequest)
{ 
    // your logic here
}
```

Extract parameters packaged in the InterjectRequest

``` CS
// parameters can be extracted directly from request into variables
string reportParam1 = interjectRequest.GetParameterValue<string>("CompanyName");
string reportParam2 = interjectRequest.GetParameterValue<string>("ContactName");
string reportParam3 = interjectRequest.GetParameterValue<string>("CustomerID");
```

Package and return response data

``` CS
// constructor accepts incoming request object
InterjectResponse response = new(interjectRequest);

// request context contains metadata
var requestContext = interjectRequest.GetRequestContext();

// XmlDataToSave contains tabular data from Excel sent by Interject
IdsTable table = requestContext.XmlDataToSave;

// perform custom logic to save incoming data

// update return data to display in Excel
table.AddColumn(new("Status"));
table.Update("Status", 1, "Updated!");

// add data to the response object
response.AddReturnedData(table);
return response;
```


# Additional Documentation

[GitHub](https://github.com/GoInterject/ids-dotnet-api/tree/main/interject.api)

[Interject Docs](https://docs.gointerject.com/)
