using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Interject.Api;

namespace Interject.DataApi
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ExampleController : ControllerBase
    {
        public ExampleController() { }

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

                // (Optional) Get DataPortal formula parameter values.
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

        /// <summary>
        /// Test endpoint for a ReportFixed function.
        /// </summary>
        /// <param name="interjectRequest">
        /// The <see cref="InterjectRequest"/> object to process.
        /// </param>
        [HttpPost("ReportFixed")]
        [ProducesResponseType(typeof(InterjectResponse), 200)]
        public InterjectResponse TestReportFixed([FromBody] InterjectRequest interjectRequest)
        {
            // Create an instance of the InterjectResponse to return by passing in the
            // InterjectRequest from the request. This copies the parameter list from
            // the request into the response.
            InterjectResponse response = new(interjectRequest);

            IdsUserContext userContext = interjectRequest.UserContext();
            string roles = userContext.UserRoles;

            try
            {
                // (Optional) Get DataPortal formula parameter values.
                string reportParam1 = interjectRequest.GetParameterValue<string>("CompanyName");
                string reportParam2 = interjectRequest.GetParameterValue<string>("ContactName");
                string reportParam3 = interjectRequest.GetParameterValue<string>("CustomerID");

                IdsTable idsTable = new();

                // (Optional) Process Column Definition Items.
                List<InterjectColDefItem> colDefItems = interjectRequest.GetColDefItems();
                foreach (var item in colDefItems)
                {
                    IdsColumn ic = new(item.ColumnName);
                    idsTable.AddColumn(ic);
                }

                // (Optional) Process Row Definition Items
                List<InterjectRowDefItem> rowDefItems = interjectRequest.GetRowDefItems();
                foreach (var item in rowDefItems)
                {
                    string div = item.GetValueString("Div");
                    string acct = item.GetValueString("Acct");

                    List<string> newRow = new()
                    {
                        div, // RowDefItem
                        acct, // RowDefItem
                        "94.34", // MTD
                        "904.34", // QTD
                        "9804.34" // YTD
                    };
                    idsTable.AddRow(newRow);
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
                // (Optional) Get DataPortal formula parameter values.
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


        /// <summary>
        /// Test endpoint for a ReportVariable function with a RowDefName column.
        /// </summary>
        /// <param name="interjectRequest">
        /// The <see cref="InterjectRequest"/> object to process.
        /// </param>
        [HttpPost("RowDef")]
        [ProducesResponseType(typeof(InterjectResponse), 200)]
        public InterjectResponse TestRowDef([FromBody] InterjectRequest interjectRequest)
        {
            // Create an instance of the InterjectResponse to return by passing in the
            // InterjectRequest from the request. This copies the parameter list from
            // the request into the response.
            InterjectResponse response = new(interjectRequest);

            var requestContext = interjectRequest.GetRequestContext();

            try
            {
                // (Optional) Get DataPortal formula parameter values.
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
                table.AddColumn(new("RowDefName"));
                table.AddColumn(new("div"));
                table.AddColumn(new("loc"));
                table.AddColumn(new("acct"));
                table.AddColumn(new("MTD"));
                table.AddColumn(new("QTD"));
                table.AddColumn(new("YTD"));
                table.AddRow(new() { "701", "701", "123", "3333", "94.30", "904.34", "9804.34" });
                table.AddRow(new() { "701", "701", "123", "3334", "94.31", "904.34", "9804.34" });
                table.AddRow(new() { "701", "701", "124", "3334", "94.32", "904.34", "9804.34" });
                table.AddRow(new() { "701", "701", "124", "3334", "94.33", "904.34", "9804.34" });
                table.AddRow(new() { "701", "701", "124", "3335", "94.34", "904.34", "9804.34" });
                table.AddRow(new() { "702", "702", "223", "3333", "94.35", "904.34", "9804.34" });
                table.AddRow(new() { "702", "702", "224", "3334", "94.36", "904.34", "9804.34" });
                table.AddRow(new() { "702", "702", "225", "3333", "94.37", "904.34", "9804.34" });
                table.AddRow(new() { "702", "702", "225", "3334", "94.38", "904.34", "9804.34" });
                table.AddRow(new() { "702", "702", "225", "3335", "94.39", "904.34", "9804.34" });

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


        /// <summary>
        /// Test endpoint for a ReportVariable function with a RowDefName column with R0000x values.
        /// </summary>
        /// <param name="interjectRequest">
        /// The <see cref="InterjectRequest"/> object to process.
        /// </param>
        [HttpPost("RowDefName")]
        [ProducesResponseType(typeof(InterjectResponse), 200)]
        public InterjectResponse TestRowDefName([FromBody] InterjectRequest interjectRequest)
        {
            // Create an instance of the InterjectResponse to return by passing in the
            // InterjectRequest from the request. This copies the parameter list from
            // the request into the response.
            InterjectResponse response = new(interjectRequest);

            var requestContext = interjectRequest.GetRequestContext();

            try
            {
                // (Optional) Get DataPortal formula parameter values.
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
                table.AddColumn(new("RowDefName"));
                table.AddColumn(new("div"));
                table.AddColumn(new("loc"));
                table.AddColumn(new("acct"));
                table.AddColumn(new("MTD"));
                table.AddColumn(new("QTD"));
                table.AddColumn(new("YTD"));
                table.AddRow(new() { "R00001", "701", "123", "3333", "94.30", "904.34", "9804.34" });
                table.AddRow(new() { "R00001", "701", "123", "3334", "94.31", "904.34", "9804.34" });
                table.AddRow(new() { "R00001", "701", "124", "3334", "94.32", "904.34", "9804.34" });
                table.AddRow(new() { "R00001", "701", "124", "3334", "94.33", "904.34", "9804.34" });
                table.AddRow(new() { "R00001", "701", "124", "3335", "94.34", "904.34", "9804.34" });
                table.AddRow(new() { "R00002", "702", "223", "3333", "94.35", "904.34", "9804.34" });
                table.AddRow(new() { "R00002", "702", "224", "3334", "94.36", "904.34", "9804.34" });
                table.AddRow(new() { "R00002", "702", "225", "3333", "94.37", "904.34", "9804.34" });
                table.AddRow(new() { "R00002", "702", "225", "3334", "94.38", "904.34", "9804.34" });
                table.AddRow(new() { "R00002", "702", "225", "3335", "94.39", "904.34", "9804.34" });
                table.AddRow(new() { "R00003", "703", "320", "3335", "94.35", "904.34", "9804.34" });
                table.AddRow(new() { "R00003", "703", "321", "3335", "94.36", "904.34", "9804.34" });
                table.AddRow(new() { "R00003", "703", "322", "3333", "94.37", "904.34", "9804.34" });
                table.AddRow(new() { "R00003", "703", "322", "3334", "94.38", "904.34", "9804.34" });
                table.AddRow(new() { "R00003", "703", "322", "3335", "94.39", "904.34", "9804.34" });

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
    }
}