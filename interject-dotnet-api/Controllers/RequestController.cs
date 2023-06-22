using Interject.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Interject.API
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RequestController : ControllerBase
    {
        public RequestController() { }

        /// <summary>
        /// Create a custom endpoint for processing the InterjectRequest and return an InterjectResponse
        /// </summary>
        /// <param name="interjectRequest">
        /// The <see cref="InterjectRequestDTO"/> object to process.
        /// </param>
        [HttpPost]
        [ProducesResponseType(typeof(InterjectResponseDTO), 200)]
        public InterjectResponseDTO Post([FromBody] InterjectRequestDTO interjectRequest)
        {
            // Create an instance of the InterjectResponseDTO to return by passing in the
            // InterjectRequestDTO from the request. This copies the parameter list from
            // the request into the response.
            InterjectResponseDTO response = new(interjectRequest);

            // (optional) Get DataPortal formula parameter values.
            string groupsCSV = interjectRequest.GetParameterValue<string>("GroupsCSV");
            int countOfRows = interjectRequest.GetParameterValue<int>("CountOfRows");

            // (optional) Process Column Definition Items.
            List<InterjectColDefItem> colDefItems = interjectRequest.GetColDefItems();
            colDefItems.ForEach(item =>
            {
                Console.WriteLine(item.ToString());
            });

            // (optional) Process Row Definition Items.
            List<InterjectRowDefItem> rowDefItems = interjectRequest.GetRowDefItems();
            rowDefItems.ForEach(item =>
            {
                Console.WriteLine(item.ToString());
            });

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

            return response;
        }
    }
}