# Example for `ReportRange`

## 1) Setting Up A Data Connection

- Ensure a data connection exists for a running data api on the interject portal - https://portal.gointerject.com/DataPortalConnections.html

For directions on how to setup a Data Connection for an API, see: https://docs.gointerject.com/wPortal/L-Api-Connections.html

## 2) Setting Up A Data Portal

- Create a new data portal on the portal site - https://portal.gointerject.com/DataPortals.html
- Add the `Interject_RequestContext` system parameter
- (Optional) Add other Formula Parameters

For directions on how to set up a Data Portal for an API, see: https://docs.gointerject.com/wPortal/Data-Portals.html

## 3) Configure the `ReportRange` Function in the Excel Report

- Configure the target data range (TargetDataRange). This is a multi-row range that defines where the data will be placed in the report.
- Configure the data range (RowDefRange). This is a single column that defines what column is used as a key to match incoming data with and what range of rows data should be populated on.
- Configure the colum definitions (ColDefRange). This range defines what columns are sent to the data api.
- (Optional) Configure the data portal parameters.

For a full working example report, see the `ReportRange` Tab in the `example.xlsx` file.

For directions on how to set up a ReportRange report, see: https://docs.gointerject.com/wGetStarted/L-Create-CustomerAging.html

For documentation on the ReportRange function, see: https://docs.gointerject.com/wIndex/ReportRange.html

## 4) Create A Post Endpoint on Data API To Process the Data Request

Here is a working example of a ReportRange endpoint:

```csharp
        /// <summary>
        /// Test endpoint for a ReportFixed function.
        /// </summary>
        /// <param name="interjectRequest">
        /// The <see cref="InterjectRequest"/> object to process.
        /// </param>
        [HttpPost("ReportRange")]
        [ProducesResponseType(typeof(InterjectResponse), 200)]
        public InterjectResponse TestReportRange([FromBody] InterjectRequest interjectRequest)
        {
            // Create an instance of the InterjectResponse to return by passing in the
            // InterjectRequest from the request. This copies the parameter list from
            // the request into the response.
            InterjectResponse response = new(interjectRequest);

            try
            {
                // (Optional) Get DataPortal formula parameter values.
                string reportParam1 = interjectRequest.GetParameterValue<string>("CompanyName");
                string reportParam2 = interjectRequest.GetParameterValue<string>("ContactName");
                string reportParam3 = interjectRequest.GetParameterValue<string>("CustomerID");

                // Add the columns
                IdsTable idsTable = new();
                idsTable.AddColumn(new IdsColumn("div"));
                idsTable.AddColumn(new IdsColumn("acct"));
                idsTable.AddColumn(new IdsColumn("MTD"));
                idsTable.AddColumn(new IdsColumn("QTD"));
                idsTable.AddColumn(new IdsColumn("YTD"));

                // Add the rows
                List<List<string>> rows = new();

                for (int i = 0; i < 10; i++)
                {
                    List<string> newRow = new() {
                        "701", "333" + i, "94.34", "904.34", "9804.34"
                    };
                    rows.Add(newRow);
                }

                for (int i = 0; i < 10; i++)
                {
                    idsTable.AddRow(rows[i]);
                }

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
