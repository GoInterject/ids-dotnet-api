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
                table.AddColumn(new("RowDefName"));
                table.AddColumn(new("Div"));
                table.AddColumn(new("Acct"));
                table.AddColumn(new("MTD"));
                table.AddColumn(new("QTD"));
                table.AddColumn(new("YTD"));
                table.AddRow(new() { "701", "701", "3333", "94.34", "904.34", "9804.34" });
                table.AddRow(new() { "701", "701", "3334", "94.34", "904.34", "9804.34" });
                table.AddRow(new() { "701", "701", "3335", "94.34", "904.34", "9804.34" });
                table.AddRow(new() { "701", "701", "3336", "94.34", "904.34", "9804.34" });
                table.AddRow(new() { "702", "702", "2333", "94.34", "904.34", "9804.34" });
                table.AddRow(new() { "702", "702", "2334", "94.34", "904.34", "9804.34" });
                table.AddRow(new() { "702", "702", "2335", "94.34", "904.34", "9804.34" });
                table.AddRow(new() { "702", "702", "2336", "94.34", "904.34", "9804.34" });
                table.AddRow(new() { "703", "703", "1333", "94.34", "904.34", "9804.34" });
                table.AddRow(new() { "703", "703", "1334", "94.34", "904.34", "9804.34" });
                table.AddRow(new() { "703", "703", "1335", "94.34", "904.34", "9804.34" });
                table.AddRow(new() { "703", "703", "1336", "94.34", "904.34", "9804.34" });
                table.AddRow(new() { "704", "704", "0333", "94.34", "904.34", "9804.34" });
                table.AddRow(new() { "704", "704", "0334", "94.34", "904.34", "9804.34" });
                table.AddRow(new() { "704", "704", "0335", "94.34", "904.34", "9804.34" });
                table.AddRow(new() { "704", "704", "0336", "94.34", "904.34", "9804.34" });

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