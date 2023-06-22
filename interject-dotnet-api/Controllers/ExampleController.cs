using Interject.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Interject.API
{
    // [Authorize] //security is currently out of scope of the project. This will be added at a later phase prior to production use.
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ExampleController : ControllerBase
    {
        public ExampleController() { }

        /// <summary>
        /// Test endpoint for a ReportSave function.
        /// </summary>
        /// <param name="interjectRequest">
        /// The <see cref="InterjectRequestDTO"/> object to process.
        /// </param>
        [HttpPost("ReportSave")]
        [ProducesResponseType(typeof(InterjectResponseDTO), 200)]
        public InterjectResponseDTO TestReportSave([FromBody] InterjectRequestDTO interjectRequest)
        {
            // Create an instance of the InterjectResponseDTO to return by passing in the
            // InterjectRequestDTO from the request. This copies the parameter list from
            // the request into the response.
            InterjectResponseDTO response = new(interjectRequest);

            try
            {
                // Get the data from the spreadsheet.
                var requestContext = interjectRequest.GetRequestContext();
                InterjectTable table = requestContext.XmlDataToSave;

                // (optional) Get DataPortal formula param values.
                string param1 = interjectRequest.GetParameterValue<string>("Param1");
                string param2 = interjectRequest.GetParameterValue<string>("Param2");

                //
                // (optional) Use data from the spreadsheet for your logic here.
                //

                // (optional) Add columns and row data for ReportSave's 'ResultsRange'
                table.AddColumn(new("Status"));
                table.Update("Status", 1, "updated!");

                response.AddReturnedData(table);

                // (optional) Show contents of Interject Table object.
                Console.Write(table.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.Write(e.StackTrace);
            }

            return response;
        }

        /// <summary>
        /// Test endpoint for a ReportFixed function.
        /// </summary>
        /// <param name="interjectRequest">
        /// The <see cref="InterjectRequestDTO"/> object to process.
        /// </param>
        [HttpPost("ReportFixed")]
        [ProducesResponseType(typeof(InterjectResponseDTO), 200)]
        public InterjectResponseDTO TestReportFixed([FromBody] InterjectRequestDTO interjectRequest)
        {
            // Create an instance of the InterjectResponseDTO to return by passing in the
            // InterjectRequestDTO from the request. This copies the parameter list from
            // the request into the response.
            InterjectResponseDTO response = new(interjectRequest);

            try
            {
                // (optional) Get DataPortal formula parameter values.
                int numberOfRowsToReturn = interjectRequest.GetParameterValue<int>("NumberOfRowsToReturn");
                int numberOfColsToReturn = interjectRequest.GetParameterValue<int>("numberOfColsToReturn");

                InterjectTable table = new();
                List<string> row = new();
                while (numberOfColsToReturn > 0)
                {
                    row.Add($"col-{numberOfColsToReturn.ToString()}-data");
                    numberOfColsToReturn--;
                }

                // (optional) Process Column Definition Items.
                List<InterjectColDefItem> colDefItems = interjectRequest.GetColDefItems();

                // int i = 1;
                // colDefItems.ForEach(item =>
                // {
                //     table.AddColumn(new(item.Value));
                //     row.Add(i.ToString());
                //     i++;
                // });


                while (numberOfRowsToReturn > 0)
                {
                    table.Rows.Add(row);
                    numberOfRowsToReturn--;
                }

                response.AddReturnedData(table);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.Write(e.StackTrace);
            }

            return response;
        }

        /// <summary>
        /// Test endpoint for a ReportVariable function.
        /// </summary>
        /// <param name="interjectRequest">
        /// The <see cref="InterjectRequestDTO"/> object to process.
        /// </param>
        [HttpPost("ReportVariable")]
        [ProducesResponseType(typeof(InterjectResponseDTO), 200)]
        public InterjectResponseDTO TestReportVariable([FromBody] InterjectRequestDTO interjectRequest)
        {
            // Create an instance of the InterjectResponseDTO to return by passing in the
            // InterjectRequestDTO from the request. This copies the parameter list from
            // the request into the response.
            InterjectResponseDTO response = new(interjectRequest);

            try
            {
                // (optional) Get DataPortal formula parameter values.
                string reportParam1 = interjectRequest.GetParameterValue<string>("Param1");
                int? reportParam2 = interjectRequest.GetParameterValue<int?>("Param2");
                int reportParam3 = interjectRequest.GetParameterValue<int>("Param3");

                // (optional) Process Column Definition Items.
                List<InterjectColDefItem> colDefItems = interjectRequest.GetColDefItems();
                colDefItems.ForEach(item =>
                {
                    Console.WriteLine(item.ToXML());
                });

                // (optional) Process Row Definition Items.
                List<InterjectRowDefItem> rowDefItems = interjectRequest.GetRowDefItems();
                rowDefItems.ForEach(item =>
                {
                    Console.WriteLine(item.ToXML());
                });

                //
                // (optional) Use data from the spreadsheet for your logic here.
                //

                // Assemble data to return to Interject to be populated on the spreadsheet.
                InterjectTable table = new();
                table.AddColumn(new("AccountType"));
                table.AddColumn(new("Segment1"));
                table.AddColumn(new("jColumnDef_1"));
                table.AddColumn(new("jColumnDef_2"));
                table.AddColumn(new("jColumnDef_3"));
                table.AddRow(new() { "Revenue - Sales", "123098", "904.34", "904.34", "904.34" });
                table.AddRow(new() { "Revenue - Sales", "123099", "1004.234", "443.14", "90.40" });
                table.AddRow(new() { "Revenue - Other", "123080", "1004.234", "443.14", "90.40" });
                table.AddRow(new() { "Revenue - Other", "123081", "1004.234", "443.14", "90.40" });

                // (optional) Show contents of Interject Table object.
                Console.Write(table.ToString());

                // (required) Add the table to the InterjectResponse object.
                response.AddReturnedData(table);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.Write(e.StackTrace);
            }

            return response;
        }
    }
}