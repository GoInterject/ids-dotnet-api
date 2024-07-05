# Example for `ReportVariable`

## 1) Setting Up A Data Connection

- Ensure a data connection exists for a running data api on the interject portal - https://portal.gointerject.com/DataPortalConnections.html

For directions on how to setup a Data Connection for an API, see: https://docs.gointerject.com/wPortal/L-Api-Connections.html

## 2) Setting Up A Data Portal

- Create a new data portal on the portal site - https://portal.gointerject.com/DataPortals.html
- Add the `Interject_RequestContext` system parameter
- (Optional) Add other Formula Parameters

For directions on how to set up a Data Portal for an API, see: https://docs.gointerject.com/wPortal/Data-Portals.html

## 3) Configure the `ReportVariable` Function in the Excel Report

- Configure the data range (RowDefRange). This is a single column that defines what column is used as a key to match incoming data with and what range of rows data should be populated on.
- Configure the column definitions (ColDefRange). This range defines what columns are sent to the data api.
- (Optional) If multiple column attributes are needed per column (i.e. financials may need Source, Period, Year, etc.), configure `jColumnDef()` and replace a column name as a string with `jColumnDef()`. For an example of this, see: https://docs.gointerject.com/wGetStarted/L-Create-FinancialVariable.html
- (Optional) Configure the data portal parameters.

For a full working example report, see the `ReportVariable` Tab in the `example.xlsx` file.

For directions on how to set up a ReportVariable report, see: https://docs.gointerject.com/wGetStarted/L-Create-InventoryVariable.html

For documentation on the ReportVariable function, see: https://docs.gointerject.com/wIndex/ReportVariable.html

## 4) Create A Post Endpoint on Data API To Process the Data Request

Here is a working example of a ReportVariable endpoint:

```csharp
        /// <summary>
        /// Test endpoint for a ReportVariable function.
        /// </summary>
        /// <param name="interjectRequest">
        /// The <see cref="InterjectRequest"/> object to process.
        /// </param>
        [HttpPost("ReportVariable")]
        [ProducesResponseType(typeof(InterjectResponse), 200)]
        public InterjectResponse TestReportVariable([FromBody] InterjectRequest interjectRequest)
        {
            // Create an instance of the InterjectResponse to return by passing in the
            // InterjectRequest from the request. This copies the parameter list from
            // the request into the response.
            InterjectResponse response = new(interjectRequest);

            var requestContext = interjectRequest.GetRequestContext();

            try
            {
                // (Optional) Get Data Portal formula parameter values.
                string reportParam1 = interjectRequest.GetParameterValue<string>("CompanyName");
                string reportParam2 = interjectRequest.GetParameterValue<string>("ContactName");
                string reportParam3 = interjectRequest.GetParameterValue<string>("CustomerID");

                // (Optional) Process Column Definition Items.
                List<InterjectColDefItem> colDefItems = interjectRequest.GetColDefItems();
                colDefItems.ForEach(item =>
                {
                    Console.WriteLine(item.ToXML());
                });

                // (Optional) Process Row Definition Items.
                List<InterjectRowDefItem> rowDefItems = interjectRequest.GetRowDefItems();
                rowDefItems.ForEach(item =>
                {
                    Console.WriteLine(item.ToXML());
                });

                foreach (InterjectColDefItem item in colDefItems)
                {
                    Console.Write(item.ToString());
                }

                // Assemble data to return to Interject to be populated on the spreadsheet.
                IdsTable table = new();
                table.AddColumn(new("div"));
                table.AddColumn(new("loc"));
                table.AddColumn(new("acct"));
                table.AddColumn(new("MTD"));
                table.AddColumn(new("QTD"));
                table.AddColumn(new("YTD"));
                table.AddRow(new() { "701", "123", "3333", "94.30", "904.34", "9804.34" });
                table.AddRow(new() { "701", "123", "3334", "94.31", "904.34", "9804.34" });
                table.AddRow(new() { "701", "124", "3334", "94.32", "904.34", "9804.34" });
                table.AddRow(new() { "701", "124", "3334", "94.33", "904.34", "9804.34" });
                table.AddRow(new() { "701", "124", "3335", "94.34", "904.34", "9804.34" });
                table.AddRow(new() { "702", "223", "3333", "94.35", "904.34", "9804.34" });
                table.AddRow(new() { "702", "224", "3334", "94.36", "904.34", "9804.34" });
                table.AddRow(new() { "702", "225", "3333", "94.37", "904.34", "9804.34" });
                table.AddRow(new() { "703", "320", "3334", "94.38", "904.34", "9804.34" });
                table.AddRow(new() { "703", "320", "3335", "94.39", "904.34", "9804.34" });
                table.AddRow(new() { "703", "321", "3333", "94.40", "904.34", "9804.34" });
                table.AddRow(new() { "703", "321", "3335", "94.41", "904.34", "9804.34" });
                table.AddRow(new() { "705", "501", "3335", "94.42", "904.34", "9804.34" });
                table.AddRow(new() { "705", "520", "3333", "94.43", "904.34", "9804.34" });
                table.AddRow(new() { "705", "520", "3334", "94.44", "904.34", "9804.34" });

                // (Optional) Show contents of Interject Table object.
                Console.Write(table.ToString());

                // (Required) Add the table to the InterjectResponse object.
                response.AddReturnedData(table);
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
