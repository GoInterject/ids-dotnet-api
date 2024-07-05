using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
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

                // (Optional) Get Data Portal formula parameter values.
                string reportParam1 = interjectRequest.GetParameterValue<string>("CompanyName");
                string reportParam2 = interjectRequest.GetParameterValue<string>("ContactName");
                string reportParam3 = interjectRequest.GetParameterValue<string>("CustomerID");
                Console.Write("P1: " + reportParam1 + "P2: " + reportParam2 + "P3: " + reportParam3);

                // (Optional) Use data from the spreadsheet for your logic here.
                // List<string> notes_list = table.GetColumnValues("Notes");

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
                // (Optional) Get Data Portal formula parameter values.
                string reportParam1 = interjectRequest.GetParameterValue<string>("CompanyName");
                string reportParam2 = interjectRequest.GetParameterValue<string>("ContactName");
                string reportParam3 = interjectRequest.GetParameterValue<string>("CustomerID");
                Console.Write("P1: " + reportParam1 + "P2: " + reportParam2 + "P3: " + reportParam3);

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

                // (Optional) Filter the table
                // string acctParam = interjectRequest.GetParameterValue<string>("account");
                // idsTable.Filter("acct", acctParam);

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
                // (Optional) Get Data Portal formula parameter values.
                string reportParam1 = interjectRequest.GetParameterValue<string>("CompanyName");
                string reportParam2 = interjectRequest.GetParameterValue<string>("ContactName");
                string reportParam3 = interjectRequest.GetParameterValue<string>("CustomerID");
                Console.Write("P1: " + reportParam1 + "P2: " + reportParam2 + "P3: " + reportParam3);

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
                    string div = item.GetValueString("div");
                    string acct = item.GetValueString("acct");

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
                // (Optional) Get Data Portal formula parameter values.
                string reportParam1 = interjectRequest.GetParameterValue<string>("CompanyName");
                string reportParam2 = interjectRequest.GetParameterValue<string>("ContactName");
                string reportParam3 = interjectRequest.GetParameterValue<string>("CustomerID");
                Console.Write("P1: " + reportParam1 + "P2: " + reportParam2 + "P3: " + reportParam3);

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
                Console.Write("P1: " + reportParam1 + "P2: " + reportParam2 + "P3: " + reportParam3);

                // (Optional) Use data from the spreadsheet for your logic here.

                // Assemble data to return to Interject to be populated on the spreadsheet
                IdsTable idsTable = new();
                idsTable.AddColumn(new IdsColumn("ytd"));
                idsTable.AddRow(new() { "123456.78" });

                // (Optional) Show contents of Interject Table object.
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
                // Assemble data to return to Interject to be populated on the spreadsheet
                IdsTable idsTable = new();
                idsTable.AddColumn(new IdsColumn("CompanyName"));
                idsTable.AddRow(new() { "Northwind Data Company" });
                idsTable.AddRow(new() { "Northwind InfoTech" });
                idsTable.AddRow(new() { "Northwind Bytes" });
                idsTable.AddRow(new() { "Northwind Security" });
                idsTable.AddRow(new() { "Northwind Research" });

                // (Optional) Show contents of Interject Table object.
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
        /// Test endpoint for Data Portal parameters.
        /// </summary>
        /// <param name="interjectRequest">
        /// The <see cref="InterjectRequest"/> object to process.
        /// </param>
        [HttpPost("DpParameters")]
        [ProducesResponseType(typeof(InterjectResponse), 200)]
        public InterjectResponse TestDPParameters([FromBody] InterjectRequest interjectRequest)
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
                Console.Write("P1: " + reportParam1 + "P2: " + reportParam2 + "P3: " + reportParam3);

                // Get Data Portal system parameter values.
                InterjectRequestContext requestContext = interjectRequest.GetRequestContext();
                String ntLogin = interjectRequest.GetParameterValue<string>("Interject_NTLogin");
                String excelVersion = interjectRequest.GetParameterValue<string>("Interject_ExcelVersion");
                String returnError = interjectRequest.GetParameterValue<string>("Interject_ReturnError");
                float localTimeZoneOffset = interjectRequest.GetParameterValue<float>("Interject_LocalTimeZoneOffset");
                String sourceFileAndPath = interjectRequest.GetParameterValue<string>("Interject_SourceFileAndPath");
                String sourceFilePathAndTab = interjectRequest.GetParameterValue<string>("Interject_SourceFilePathAndTab");
                String userID = interjectRequest.GetParameterValue<string>("Interject_UserID");
                String loginName = interjectRequest.GetParameterValue<string>("Interject_LoginName");
                String userRoles = interjectRequest.GetParameterValue<string>("Interject_UserRoles");
                String clientID = interjectRequest.GetParameterValue<string>("Interject_ClientID");

                // Get Data Portal system parameter values from RequestContext parameter
                IdsTable saveDataTable = interjectRequest.GetXmlDataToSave();
                List<InterjectRowDefItem> rowDefItems = interjectRequest.GetRowDefItems();
                List<InterjectColDefItem> colDefItems = interjectRequest.GetColDefItems();
                String excelVersion2 = interjectRequest.GetRequestContext().ExcelVersion;
                String userID2 = interjectRequest.GetRequestContext().UserContext.UserId;
                String clientID2 = interjectRequest.GetRequestContext().UserContext.ClientId;
                String loginName2 = interjectRequest.GetRequestContext().UserContext.LoginName;
                String machineLoginName = interjectRequest.GetRequestContext().UserContext.MachineLoginName;
                String machineName = interjectRequest.GetRequestContext().UserContext.MachineName;
                String fullName = interjectRequest.GetRequestContext().UserContext.FullName;
                String loginDateUtc = interjectRequest.GetRequestContext().UserContext.LoginDateUtc;
                String userRoles2 = interjectRequest.GetRequestContext().UserContext.UserRoles;
                String idsVersion = interjectRequest.GetRequestContext().IdsVersion;
                String fileName = interjectRequest.GetRequestContext().FileName;
                String filePath = interjectRequest.GetRequestContext().FilePath;
                String tabName = interjectRequest.GetRequestContext().TabName;
                String cellRange = interjectRequest.GetRequestContext().CellRange;
                String sourceFunction = interjectRequest.GetRequestContext().SourceFunction;
                String utcOffset = interjectRequest.GetRequestContext().UtcOffset;

                // Build strings of rowDefItems and colDefItems
                StringBuilder rowDefItemsStringBuilder = new StringBuilder();
                foreach (InterjectRowDefItem item in rowDefItems)
                {
                    rowDefItemsStringBuilder.Append("; ").Append(item.ToString());
                }
                string rowDefItemsString = rowDefItemsStringBuilder.ToString();

                StringBuilder colDefItemsStringBuilder = new StringBuilder();
                foreach (InterjectColDefItem item in colDefItems)
                {
                    colDefItemsStringBuilder.Append("; ").Append(item.ToString());
                }
                string colDefItemsString = colDefItemsStringBuilder.ToString();

                // Print the parameters
                Console.WriteLine($"rowDefItems: {rowDefItemsString}");
                Console.WriteLine($"colDefItems: {colDefItemsString}");
                Console.WriteLine($"ntLogin: {ntLogin}");
                Console.WriteLine($"excelVersion: {excelVersion}");
                Console.WriteLine($"returnError: {returnError}");
                Console.WriteLine($"localTimeZoneOffset: {localTimeZoneOffset}");
                Console.WriteLine($"sourceFileAndPath: {sourceFileAndPath}");
                Console.WriteLine($"sourceFilePathAndTab: {sourceFilePathAndTab}");
                Console.WriteLine($"userID: {userID}");
                Console.WriteLine($"loginName: {loginName}");
                Console.WriteLine($"userRoles: {userRoles}");
                Console.WriteLine($"clientID: {clientID}");
                Console.WriteLine($"excelVersion2: {excelVersion2}");
                Console.WriteLine($"userID2: {userID2}");
                Console.WriteLine($"clientID2: {clientID2}");
                Console.WriteLine($"loginName2: {loginName2}");
                Console.WriteLine($"machineLoginName: {machineLoginName}");
                Console.WriteLine($"machineName: {machineName}");
                Console.WriteLine($"fullName: {fullName}");
                Console.WriteLine($"loginDateUtc: {loginDateUtc}");
                Console.WriteLine($"userRoles2: {string.Join(", ", userRoles2)}");
                Console.WriteLine($"idsVersion: {idsVersion}");
                Console.WriteLine($"fileName: {fileName}");
                Console.WriteLine($"filePath: {filePath}");
                Console.WriteLine($"tabName: {tabName}");
                Console.WriteLine($"cellRange: {cellRange}");
                Console.WriteLine($"sourceFunction: {sourceFunction}");
                Console.WriteLine($"utcOffset: {utcOffset}");

                // Assemble a data table with the parameters to be populated on the spreadsheet
                IdsTable idsTable = new();
                idsTable.AddColumn(new IdsColumn("Parameter"));
                idsTable.AddColumn(new IdsColumn("Value"));

                idsTable.AddRow(new() { "RowDefItems", rowDefItemsString });
                idsTable.AddRow(new() { "ColDefItems", colDefItemsString });
                idsTable.AddRow(new() { "ntLogin", ntLogin });
                idsTable.AddRow(new() { "excelVersion", excelVersion });
                idsTable.AddRow(new() { "returnError", returnError });
                idsTable.AddRow(new() { "localTimeZoneOffset", localTimeZoneOffset.ToString() });
                idsTable.AddRow(new() { "sourceFileAndPath", sourceFileAndPath });
                idsTable.AddRow(new() { "sourceFilePathAndTab", sourceFilePathAndTab });
                idsTable.AddRow(new() { "userID", userID });
                idsTable.AddRow(new() { "loginName", loginName });
                idsTable.AddRow(new() { "userRoles", userRoles });
                idsTable.AddRow(new() { "clientID", clientID });
                idsTable.AddRow(new() { "excelVersion2", excelVersion2 });
                idsTable.AddRow(new() { "userID2", userID2 });
                idsTable.AddRow(new() { "clientID2", clientID2 });
                idsTable.AddRow(new() { "loginName2", loginName2 });
                idsTable.AddRow(new() { "machineLoginName", machineLoginName });
                idsTable.AddRow(new() { "machineName", machineName });
                idsTable.AddRow(new() { "fullName", fullName });
                idsTable.AddRow(new() { "loginDateUtc", loginDateUtc });
                idsTable.AddRow(new() { "userRoles2", string.Join(", ", userRoles2) });
                idsTable.AddRow(new() { "idsVersion", idsVersion });
                idsTable.AddRow(new() { "fileName", fileName });
                idsTable.AddRow(new() { "filePath", filePath });
                idsTable.AddRow(new() { "tabName", tabName });
                idsTable.AddRow(new() { "cellRange", cellRange });
                idsTable.AddRow(new() { "sourceFunction", sourceFunction });
                idsTable.AddRow(new() { "utcOffset", utcOffset });

                // (Optional) Show contents of Interject Table object.
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
                // (Optional) Get Data Portal formula parameter values.
                string reportParam1 = interjectRequest.GetParameterValue<string>("CompanyName");
                string reportParam2 = interjectRequest.GetParameterValue<string>("ContactName");
                string reportParam3 = interjectRequest.GetParameterValue<string>("CustomerID");
                Console.Write("P1: " + reportParam1 + "P2: " + reportParam2 + "P3: " + reportParam3);

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
                // (Optional) Get Data Portal formula parameter values.
                string reportParam1 = interjectRequest.GetParameterValue<string>("CompanyName");
                string reportParam2 = interjectRequest.GetParameterValue<string>("ContactName");
                string reportParam3 = interjectRequest.GetParameterValue<string>("CustomerID");
                Console.Write("P1: " + reportParam1 + "P2: " + reportParam2 + "P3: " + reportParam3);

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