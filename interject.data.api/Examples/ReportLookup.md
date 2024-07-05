# Example for `ReportLookup`

## 1) Setting Up A Data Connection

- Ensure a data connection exists for a running data api on the interject portal - https://portal.gointerject.com/DataPortalConnections.html

For directions on how to setup a Data Connection for an API, see: https://docs.gointerject.com/wPortal/L-Api-Connections.html

## 2) Setting Up A Data Portal

- Create a new data portal on the portal site - https://portal.gointerject.com/DataPortals.html
- Add the `Interject_RequestContext` system parameter
- (Optional) Add other Formula Parameters

For directions on how to set up a Data Portal for an API, see: https://docs.gointerject.com/wPortal/Data-Portals.html

## 3) Configure the `ReportLookup` Function in the Excel Report

- Configure the target data range (TargetDataRange). This is a multi-row range that defines where the data will be placed in the report.
- Configure the colum definition (ColDefRange). This is a single cell that defines the column of the data source.
- (Optional) Configure the data portal parameters.

For a full working example report, see the `ReportLookup` Tab in the `example.xlsx` file.

For documentation on the ReportLookup function, see: https://docs.gointerject.com/wIndex/ReportLookup.html

## 4) Create A Post Endpoint on Data API To Process the Data Request

Here is a working example of a ReportLookup endpoint:

```csharp
        /// <summary>
        /// Test endpoint for a ReportLookup function.
        /// </summary>
        /// <param name="interjectRequest">
        /// The <see cref="InterjectRequest"/> object to process.
        /// </param>
        [HttpPost("ReportLookup")]
        [ProducesResponseType(typeof(InterjectResponse), 200)]
        public InterjectResponse TestReportLookup([FromBody] InterjectRequest interjectRequest)
        {
            // Create an instance of the InterjectResponse to return by passing in the
            // InterjectRequest from the request. This copies the parameter list from
            // the request into the response.
            InterjectResponse response = new(interjectRequest);

            try
            {
                // (Optional) Get Data Portal formula parameter values.
                string reportParam1 = interjectRequest.GetParameterValue<string>("CompanyName");
                string reportParam2 = interjectRequest.GetParameterValue<string>("ContactName");
                string reportParam3 = interjectRequest.GetParameterValue<string>("CustomerID");

                // Add the columns
                IdsTable idsTable = new();
                idsTable.AddColumn(new IdsColumn("ytd"));
                idsTable.AddRow(new() { "123456.78" });

                idsTable.Print();

                response.AddReturnedData(idsTable);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.Write(e.StackTrace);
                response.ErrorMessage = e.Message;
            }

            return response;
        }
```
