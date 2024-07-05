# Example for `ReportSave`

## 1) Setting Up A Data Connection

- Ensure a data connection exists for a running data api on the interject portal - https://portal.gointerject.com/DataPortalConnections.html

For directions on how to setup a Data Connection for an API, see: https://docs.gointerject.com/wPortal/L-Api-Connections.html

## 2) Setting Up A Data Portal

- Create a new data portal on the portal site - https://portal.gointerject.com/DataPortals.html
- Add the `Interject_XMLDataToSave` system parameter
- (Optional) Add other Formula Parameters

For directions on how to set up a Data Portal for an API, see: https://docs.gointerject.com/wPortal/Data-Portals.html

## 3) Configure the `ReportSave` Function in the Excel Report

- Configure the data range (RowDefRange). This is a single column that defines the unique row IDs taht will be used to save the rows.
- Configure the column definitions (ColDefRange). This range defines what columns are sent to the data api.
- (Optional) Configure the results range colums (ResultsRange). This range defines what columns are populated when the data is returned.
- (Optional) Configure the data portal parameters.

For a full working example report, see the `ReportSave` Tab in the `example.xlsx` file.

For directions on how to set up a ReportSave report, see: https://docs.gointerject.com/wGetStarted/L-Dev-CustomerCreditSave.html

For documentation on the ReportSave function, see: https://docs.gointerject.com/wIndex/ReportSave.html

## 4) Create A Post Endpoint on Data API To Process the Save Data

Here is a working example of a ReportSave endpoint:

```csharp
        /// <summary>
        /// Test endpoint for a ReportSave function.
        /// </summary>
        /// <param name="interjectRequest">
        /// The <see cref="InterjectRequest"/> object to process.
        /// </param>
        [HttpPost("ReportSave")]
        [ProducesResponseType(typeof(InterjectResponse), 200)]
        public InterjectResponse TestReportSave([FromBody] InterjectRequest interjectRequest)
        {
            // Create an instance of the InterjectResponse to return by passing in the
            // InterjectRequest from the request. This copies the parameter list from
            // the request into the response.
            InterjectResponse response = new(interjectRequest);

            try
            {
                // Get the data from the spreadsheet.
                var requestContext = interjectRequest.GetRequestContext();

                IdsTable table = requestContext.XmlDataToSave;

                // (Optional) Get Data Portal formula parameter values.
                string reportParam1 = interjectRequest.GetParameterValue<string>("CompanyName");
                string reportParam2 = interjectRequest.GetParameterValue<string>("ContactName");
                string reportParam3 = interjectRequest.GetParameterValue<string>("CustomerID");

                // (Optional) Use data from the spreadsheet for your logic here.
                //List<string> list = table.GetColumnValues("Notes");

                // (Optional) Add columns and row data for ReportSave's 'ResultsRange'
                table.AddColumn(new("Status"));
                table.Update("Status", 1, "Updated!");

                response.AddReturnedData(table);

                // (Optional) Show contents of Interject Table object.
                Console.Write(table.ToString());
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
