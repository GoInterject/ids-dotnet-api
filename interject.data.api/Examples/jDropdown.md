# Example for `jDropdown`

## 1) Setting Up A Data Connection

- Ensure a data connection exists for a running data api on the interject portal - https://portal.gointerject.com/DataPortalConnections.html

For directions on how to setup a Data Connection for an API, see: https://docs.gointerject.com/wPortal/L-Api-Connections.html

## 2) Setting Up A Data Portal

- Create a new data portal on the portal site - https://portal.gointerject.com/DataPortals.html
- Add the `Interject_RequestContext` system parameter
- (Optional) Add other Formula Parameters

For directions on how to set up a Data Portal for an API, see: https://docs.gointerject.com/wPortal/Data-Portals.html

## 3) Configure the `jDropdown` Function in the Excel Report

- Configure the Target Cell. This is the cell that the data will be inserted in.
- Configure the Value Column Name. This is the name of the column returned to Interject from the data source.
- (Optional) Configure the data portal parameters.

For a full working example report, see the `jDropdown` Tab in the `example.xlsx` file.

For directions on how to set up a jDropdown, see: https://docs.gointerject.com/wGetStarted/L-Create-Dropdowns.html

For documentation on the jDropdown function, see: https://docs.gointerject.com/wIndex/jDropdown.html

## 4) Create A Post Endpoint on Data API To Process the Data Request

Here is a working example of a jDropdown endpoint:

```csharp
        /// <summary>
        /// Test endpoint for a ReportLookup function.
        /// </summary>
        /// <param name="interjectRequest">
        /// The <see cref="InterjectRequest"/> object to process.
        /// </param>
        [HttpPost("jDropdown")]
        [ProducesResponseType(typeof(InterjectResponse), 200)]
        public InterjectResponse TestJDropdown([FromBody] InterjectRequest interjectRequest)
        {
            // Create an instance of the InterjectResponse to return by passing in the
            // InterjectRequest from the request. This copies the parameter list from
            // the request into the response.
            InterjectResponse response = new(interjectRequest);

            try
            {
                // Add the columns
                IdsTable idsTable = new();
                idsTable.AddColumn(new IdsColumn("CompanyName"));
                idsTable.AddRow(new() { "Northwind Data Company" });
                idsTable.AddRow(new() { "Northwind InfoTech" });
                idsTable.AddRow(new() { "Northwind Bytes" });
                idsTable.AddRow(new() { "Northwind Security" });
                idsTable.AddRow(new() { "Northwind Research" });

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
