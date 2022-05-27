using Interject.Classes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Interject.API
{
    // [Authorize] //security is currently out of scope of the project. This will be added at a later phase prior to production use.
    [ApiController]
    [Route("api/v1/[controller]")]
    public class NISCController : ControllerBase
    {
        private readonly InterjectRequestHandler _requestHandler;

        public NISCController(InterjectRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        /// <summary>
        /// PL report with account level detail.
        /// </summary>
        /// <param name="idsRequest">The <see cref="InterjectRequest"/> object sent from the Interject addin.</param>
        /// <returns><see cref="InterjectResponse"/></returns>
        [HttpPost("PLDetail")]
        public InterjectResponse PostPLDetail(InterjectRequest idsRequest)
        {
            // An example of securing endponts by subdomain.
            // This is not currently enforced.
            var tenantCode = GetTenantCode(HttpContext.Request.Host.Host);

            _requestHandler.Init(idsRequest);
            _requestHandler.ConvertParameters(new PLDetailAndSummaryParameterConverter());
            _requestHandler.FetchData(new ExampleDataConnection(tenantCode, "PLDetail"));
            _requestHandler.ConvertResponseData(new CSVResponseConverter());
            return _requestHandler.PackagedResponse;
        }

        /// <summary>
        /// PL report at summary level.
        /// </summary>
        /// <param name="idsRequest">The <see cref="InterjectRequest"/> object sent from the Interject addin.</param>
        /// <returns><see cref="InterjectResponse"/></returns>
        [HttpPost("PLSummary")]
        public InterjectResponse PostPLSummary(InterjectRequest idsRequest)
        {
            var tenantCode = GetTenantCode(HttpContext.Request.Host.Host);

            _requestHandler.Init(idsRequest);
            _requestHandler.ConvertParameters(new PLDetailAndSummaryParameterConverter());
            _requestHandler.FetchData(new ExampleDataConnection(tenantCode, "PLSummary"));
            _requestHandler.ConvertResponseData(new CSVResponseConverter());
            return _requestHandler.PackagedResponse;
        }

        /// <summary>
        /// Full account level detail for drill from summary report.
        /// </summary>
        /// <param name="idsRequest">The <see cref="InterjectRequest"/> object sent from the Interject addin.</param>
        /// <returns><see cref="InterjectResponse"/></returns>
        [HttpPost("AccountDetail")]
        public InterjectResponse PostAccountDetail(InterjectRequest idsRequest)
        {
            var tenantCode = GetTenantCode(HttpContext.Request.Host.Host);

            _requestHandler.Init(idsRequest);
            _requestHandler.ConvertParameters(new AccountDetailParameterConverter());
            _requestHandler.FetchData(new ExampleDataConnection(tenantCode, "AccountDetail"));
            _requestHandler.ConvertResponseData(new CSVResponseConverter());
            return _requestHandler.PackagedResponse;
        }

        /// <summary>
        /// Parses the host string from the HttpRequest and returns the tenant code which is the subdomain (host).
        /// </summary>
        /// <param name="host">The host string from the HttpRequest.</param>
        /// <returns><see cref="string"/>: Tenant Code</returns>
        /// <exception cref="UserException"></exception>
        private static string GetTenantCode(string host)
        {
            if (string.IsNullOrEmpty(host))
            {
                throw new UserException("[0] No tenant code not found in request.");
            }
            var code = string.Empty;
            var nodes = host.Split('.');

            // Do not accept "www" as a code.
            if (nodes.Length == 1)
            {
                if (string.Equals(nodes[0], "www", StringComparison.OrdinalIgnoreCase))
                {
                    throw new UserException("[1] No tenant code not found in request.");
                }
                else
                {
                    code = nodes[0];
                }
            }
            else
            {
                if (string.Equals(nodes[0], "www", StringComparison.OrdinalIgnoreCase))
                {
                    code = nodes[1];
                }
                else
                {
                    code = nodes[0];
                }
            }

            if (string.IsNullOrEmpty(code))
            {
                throw new UserException("[2] No tenant code not found in request.");
            }
            else
            {
                return code;
            }
        }

        public class PLDetailAndSummaryParameterConverter : IParameterConverter
        {
            public void Convert(InterjectRequestHandler handler)
            {
                var ym = GetYearMonthParameter(handler.IdsRequest.RequestParameterList);

                handler.IdsResponse.RequestParameterList = new List<RequestParameter>();
                handler.IdsResponse.RequestParameterList.Add(ym);
            }
        }

        public class AccountDetailParameterConverter : IParameterConverter
        {
            public void Convert(InterjectRequestHandler handler)
            {
                var ym = GetYearMonthParameter(handler.IdsRequest.RequestParameterList);

                string selectedGroupCode = "";
                var gcParam = handler.IdsRequest.RequestParameterList.FirstOrDefault(a => a.Name == "@GroupCode");
                if (gcParam != null)
                {
                    selectedGroupCode = gcParam.InputValue;
                }

                RequestParameter gc = new()
                {
                    Name = "@GroupCode",
                    DataType = ParameterDataType.varchar,
                    ExpectsOutput = true,
                    InputValue = selectedGroupCode,
                    OutputValue = selectedGroupCode
                };

                handler.IdsResponse.RequestParameterList = new List<RequestParameter>();
                handler.IdsResponse.RequestParameterList.Add(ym);
                handler.IdsResponse.RequestParameterList.Add(gc);
            }
        }

        private static RequestParameter GetYearMonthParameter(List<RequestParameter> requestParameters)
        {
            string selectedYearMonth = "";
            var ymParm = requestParameters.FirstOrDefault(a => a.Name == "@YearMonth");
            if (ymParm != null)
            {
                if (ymParm.InputValue != null)
                {
                    selectedYearMonth = ymParm.InputValue;
                }
            }

            if (selectedYearMonth.Length == 0) // If not entered or blank, default to current close month.
            {
                selectedYearMonth = DateTime.Now.AddDays(-10).ToString("yyyy-MM");
            }

            RequestParameter ym = new()
            {
                Name = "@YearMonth",
                DataType = ParameterDataType.varchar,
                ExpectsOutput = true,
                InputValue = selectedYearMonth,
                OutputValue = selectedYearMonth
            };

            return ym;
        }

        public class ExampleDataConnection : IDataConnection
        {
            private string _tableName { get; set; }
            private string _tenantCode { get; set; }
            private InterjectTable _table { get; set; }
            public ExampleDataConnection(string tenantCode, string tableName)
            {
                _tableName = tableName;
                _tenantCode = tenantCode;
            }

            public void FetchData(InterjectRequestHandler handler)
            {
                var yearMonth = handler.IdsResponse.RequestParameterList.FirstOrDefault(a => a.Name == "@YearMonth").InputValue;
                ExampleDataDTO result = new();
                switch (_tableName)
                {
                    case "PLDetail":
                        result.Table = ExampleData.PLDetailTable;
                        result.CSVString = ExampleData.NISC_PLDetail(_tenantCode, yearMonth);
                        handler.ReturnData = new List<ExampleDataDTO>() { result };
                        return;
                    case "PLSummary":
                        result.Table = ExampleData.PLSummaryTable;
                        result.CSVString = ExampleData.NISC_PLSummary(_tenantCode, yearMonth);
                        handler.ReturnData = new List<ExampleDataDTO>() { result };
                        return;
                    case "AccountDetail":
                        result.Table = ExampleData.PLDetailTable;
                        var groupCode = handler.IdsResponse.RequestParameterList.FirstOrDefault(a => a.Name == "@GroupCode").InputValue;
                        result.CSVString = ExampleData.NISC_PLDetail(_tenantCode, yearMonth, groupCode);
                        handler.ReturnData = new List<ExampleDataDTO>() { result };
                        return;
                    default:
                        throw new UserException("Invalid or missing table name in ExampleDataConnection.FetchData.");
                }
            }

            public async Task FetchDataAsync(InterjectRequestHandler handler)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// A Data Transfer Object for collecting the example data components.<br/>
        /// The InterjectTable and CSV string are held here for later processing
        /// when the string will be converted into row data and added to the table.
        /// </summary>
        public class ExampleDataDTO
        {
            /// <summary>
            /// Fully formed table containing the rows for the table data.
            /// </summary>
            public InterjectTable Table { get; set; }
            /// <summary>
            /// A pipe delimited value string containing table data.
            /// </summary>
            public string CSVString { get; set; }
        }

        public class CSVResponseConverter : IResponseConverter
        {
            public void Convert(InterjectRequestHandler handler)
            {
                var exampleDtos = handler.ReturnData as List<ExampleDataDTO>;
                exampleDtos.ForEach((dto) =>
                {
                    dto.Table.Rows = CsvToRows(dto.CSVString);
                    ReturnedData rd = new(dto.Table);
                    handler.IdsResponse.ReturnedDataList.Add(rd);
                });
            }

            private List<List<object>> CsvToRows(string dataString)
            {
                List<List<object>> result = new();
                using (StringReader reader = new StringReader(dataString))
                {
                    string line = string.Empty;
                    do
                    {
                        line = reader.ReadLine();
                        if (line != null)
                        {
                            // use | in example data to avoid unintended delimiters
                            var values = line.Split('|');
                            List<object> Row = new List<object>();
                            foreach (string item in values)
                            {
                                Row.Add(item);
                            }
                            result.Add(Row);
                        }
                    } while (line != null);
                }
                return result;
            }
        }

        /// <summary>
        /// This class holds data strings to provide for example reports.<br/>
        /// In production, the data will be sourced from a database.
        /// </summary>
        public class ExampleData
        {
            public static InterjectTable PLDetailTable
            {
                get
                {
                    InterjectTable t = new();
                    t.TableName = ""; // Not required to be entered.
                                      //add in the same order as the data provided in this example
                                      //data types are typical c# types that can map to a database like SQL Server.
                    t.AddColumn(new("GroupCode", "String"));
                    t.AddColumn(new("GroupName", "String"));
                    t.AddColumn(new("Seg1", "String"));
                    t.AddColumn(new("Seg2", "String"));
                    t.AddColumn(new("SegName", "String"));
                    t.AddColumn(new("LY_YTD", "Decimal"));
                    t.AddColumn(new("CY_YTD", "Decimal"));
                    t.AddColumn(new("BUD_YTD", "Decimal"));
                    t.AddColumn(new("CY_MTD", "Decimal"));
                    t.AddColumn(new("BUD_MTD", "Decimal"));
                    return t;
                }
            }

            public static InterjectTable PLSummaryTable
            {
                get
                {
                    InterjectTable t = new();
                    t.AddColumn(new("GroupCode", "String"));
                    t.AddColumn(new("GroupName", "String"));
                    t.AddColumn(new("LY_YTD", "Decimal"));
                    t.AddColumn(new("CY_YTD", "Decimal"));
                    t.AddColumn(new("BUD_YTD", "Decimal"));
                    t.AddColumn(new("CY_MTD", "Decimal"));
                    t.AddColumn(new("BUD_MTD", "Decimal"));
                    return t;
                }
            }

            /// <summary>
            /// The parameters are for example only since no database request is included.
            /// </summary>
            /// <param name="tenantCode"></param>
            /// <param name="YearMonth"></param>
            /// <param name="selectedGroupCode"></param>
            /// <returns></returns>
            public static string NISC_PLDetail(string tenantCode, string YearMonth, string selectedGroupCode = "")
            {
                return @"1|1. Local Network Services Revenue|10|5001|BASIC LOCAL SERVICE REVENUE|0|0|0|0|0
1|1. Local Network Services Revenue|10|5001.1|BASIC LOCAL SERVICE REVENUE-RESIDE|1432986.25|1290904.52|1294500|430581.41|427000
1|1. Local Network Services Revenue|10|5001.11|BASIC LOCAL SERVICE REVENUE-BUSIN|414540.95|417149.93|399000|135810.18|132500
1|1. Local Network Services Revenue|10|5001.3|BASIC LS SERV REV - PIC CHANGE|85|150|0|35|0
1|1. Local Network Services Revenue|10|5001.4|BUNDLE CREDIT - VOICE SERVICE|0|-86548.51|-57600|-30353.59|-19200
1|1. Local Network Services Revenue|10|5040.1|LOCAL PRIVATE LINE REV-SPEC|46644.34|49347.72|48000|16490.16|16000
1|1. Local Network Services Revenue|10|5040.11|LOCAL PRIVATE LINE REV-ISDN|46439.17|44969.51|45000|14745.61|15000
1|1. Local Network Services Revenue|10|5040.2|LOCAL PRIVATE LINE REV-BLTC & NRTC|11058.14|10010.13|10500|3320.07|3500
1|1. Local Network Services Revenue|10|5060.1|OTHER LOCAL EXCHANGE REV-RESIDEN|288254.5|261416.29|260250|87035.77|86000
1|1. Local Network Services Revenue|10|5060.11|OTHER LOCAL EXCHANGE REV-BUSINES|52663.02|50697.09|50550|16896.87|16800
1|1. Local Network Services Revenue|22|5001.1|BASIC LOCAL SERVICE REVENUE-RESIDE|1638.7|3055.94|3600|1028.19|1300
1|1. Local Network Services Revenue|22|5001.11|BASIC LOCAL SERVICE REVENUE-BUSIN|138691.83|159614.84|165600|53924.03|55600
1|1. Local Network Services Revenue|22|5001.4|BUNDLE CREDIT - VOICE SERVICE|0|-8951.95|-2100|-3026.83|-700
1|1. Local Network Services Revenue|22|5040.11|LOCAL PRIVATE LINE-BUSINESS|0|32690.31|39800|10896.77|13500
1|1. Local Network Services Revenue|22|5040.14|LOCAL PRIVATE LINE REV-BUSINESS|27249.11|0|0|0|0
1|1. Local Network Services Revenue|22|5060.1|OTHER LOCAL EXCHANGE REV-CLASS SE|43.86|354.14|600|116.2|200
1|1. Local Network Services Revenue|22|5060.11|OTHER LOCAL EXCHANGE REV-BUSINES|18372.67|21315.29|21900|7219.87|7400
1|1. Local Network Services Revenue|26|5001.1|BASIC LOCAL SERVICE REVENUE-RESIDE|2858.54|3124.32|3500|1181.1|1200
1|1. Local Network Services Revenue|26|5001.11|BASIC LOCAL SERVICE REVENUE-BUSIN|522.72|510.72|600|170.24|200
1|1. Local Network Services Revenue|26|5001.4|BUNDLE CREDIT - VOICE SERVICE|0|-347.1|-300|-126.7|-100
1|1. Local Network Services Revenue|26|5040.11|LOCAL PRIVATE LINE REV-BUSINESS|0|3622.5|3600|1207.5|1200
1|1. Local Network Services Revenue|26|5040.14|LOCAL PRIVATE LINE REV-BUSINESS|3622.5|0|0|0|0
1|1. Local Network Services Revenue|26|5060.1|CALLING FEATURES|261.62|383.36|300|150.06|100
1|1. Local Network Services Revenue|26|5060.11|OTHER LOCAL EXCHANGE REV-BUSINES|217.38|217.38|300|72.46|100
1|1. Local Network Services Revenue|30|5001.1|BASIC LOCAL SERVICE REVENUE-RESIDE|0|0|0|0|0
1|1. Local Network Services Revenue|30|5001.11|BASIC LOCAL SERVICE REVENUE-BUSIN|0|0|0|0|0
1|1. Local Network Services Revenue|30|5810|WIRELESS BACKUP SERVICE|10281|10281|10200|3427|3400
10|10. Depreciation Expense|10|6560|DEPRECIATION AND AMORTIZATION EXP|0|0|0|0|0
10|10. Depreciation Expense|10|6561.1|DEPR EXP-TELECOM PLANT IN SERV|2955633.05|1444300.21|2442000|0|814000
10|10. Depreciation Expense|10|6561.1009|DEPR EXP-TELECOM PLANT IN SERV-BIP/|213224.55|142149.7|214500|0|71500
10|10. Depreciation Expense|10|6561.2|DEPRECIATION EXPENSE-MOTOR VEHICL|78160.59|54841.95|83100|0|27700
10|10. Depreciation Expense|20|6561.1|DEPR EXP-TELECOM PLANT IN SERV|25526.12|14472.73|25200|0|8400
10|10. Depreciation Expense|21|6561.1|DEPR EXP-TELECOM PLANT IN SERV|136200|86757.11|151290|0|50430
10|10. Depreciation Expense|22|6561.1|DEPR EXP-TELECOM PLANT IN SERV|111429.22|89600.22|163260|0|54420
10|10. Depreciation Expense|25|6561.1|DEPR EXP-TELECOM PLANT IN SERV|0|3489.52|6900|0|2300
10|10. Depreciation Expense|26|6561.1|DEPR EXP-TELECOM PLANT IN SERV|4914.9|5495.39|7050|0|2350
10|10. Depreciation Expense|30|6561.1|DEPR EXP-TELECOM PLANT IN SERV|3474.78|1704.88|2400|0|800
10|10. Depreciation Expense|31|6561.1009|DEPR EXP-TELECOM PLANT IN SERV-BIP/|52702.26|35134.84|52700|0|17550
11|11. Amortization Expense|30|6563.1|AMORTIZATION EXPENSE|16468.08|10978.72|16470|0|5485
11|11. Amortization Expense|31|6563.1|AMORTIZATION EXPENSE|13012.11|8674.74|13030|0|4340
12|12. Customer Operations Expense|10|6611.1|SALES COMMISSIONS EXPENSE|166.67|4779.75|14825|0|4925
12|12. Customer Operations Expense|10|6612.1|SALES/MARKETING|75330.52|74992.72|144225|27301.36|47875
12|12. Customer Operations Expense|10|6613.1|PRODUCT ADVERTISING|51521.03|34882.82|73050|11291.71|24800
12|12. Customer Operations Expense|10|6613.11|WEBSITE EXPENSES|363.06|0|0|0|0
12|12. Customer Operations Expense|10|6622.2|NUMBER SERVICES-DIRECTORY ASSISTA|2831.19|2421.42|4800|844.66|1600
12|12. Customer Operations Expense|10|6623.1|CUSTOMER SERV-COMM-REQUEST PROC|97666.9|95508.04|112600|28286|28500
12|12. Customer Operations Expense|10|6623.11|CUSTOMER SERV-COMM PYMT & COLLE|233387.55|207663.65|211800|88895.26|67400
12|12. Customer Operations Expense|10|6623.12|CUSTOMER SER-COMMERCIAL-BILL INQ|31686.49|25237.19|40100|9118.7|13000
12|12. Customer Operations Expense|10|6623.2|CUSTOMER SERV-BILLING CONTROL|49295.12|43713.07|54800|14654.83|17100
12|12. Customer Operations Expense|10|6623.3|CUSTOMER SERV-TRS|53954.79|30798.03|31500|10266.01|10500
12|12. Customer Operations Expense|10|6623.4|CUSTOMER SERV-DATA PROCESSING|316.29|0|0|0|0
12|12. Customer Operations Expense|10|6623.41|CUSTOMER SERV-DP-BILL&COLLECT RE|15971.96|14881.81|17800|4632.52|3600
12|12. Customer Operations Expense|10|6623.42|CUSTOMER SERV-BILLING CONTRACT|24380.35|23509.78|27600|7813.07|9200
12|12. Customer Operations Expense|10|6623.43|CUSTOMER SERV-MESSAGE TOLL PROCE|682.5|682.5|0|227.5|0
12|12. Customer Operations Expense|10|6623.5|CUSTOMER SERV-CARRIER ACCESS BILLI|6227.8|3742.75|7600|1428.96|2400
12|12. Customer Operations Expense|10|6623.55|CUSTOMER SERV-GENERAL ADMINISTRA|93929.35|85733.47|90300|34443.02|24500
12|12. Customer Operations Expense|10|6623.9|COST OF SALES|0|0|0|0|0
12|12. Customer Operations Expense|10|8111.6|WIRELESS COST OF GOODS SOLD|879.11|620.26|0|0|0
12|12. Customer Operations Expense|20|6612.1|SALES/MARKETING|0|0|0|0|0
12|12. Customer Operations Expense|20|6613.1|PRODUCT ADVERTISING|83.77|0|0|0|0
12|12. Customer Operations Expense|20|6613.11|WEBSITE EXPENSES|0|0|0|0|0
12|12. Customer Operations Expense|20|6623|CUSTOMER BILLING EXPENSE|0|0|0|0|0
12|12. Customer Operations Expense|20|6623.45|CUSTOMER SERVICE-TECHNICAL SUPPO|9498.93|2654.4|2000|592.88|100
12|12. Customer Operations Expense|20|6623.55|CUSTOMER SERV-GENERAL ADMINISTRA|1677.42|1524.21|3600|519.81|900
12|12. Customer Operations Expense|20|6623.9|COST OF SALES|0|0|0|0|0
12|12. Customer Operations Expense|21|6612.1|SALES/MARKETING|0|0|0|0|0
12|12. Customer Operations Expense|21|6613.1|PRODUCT ADVERTISING|13179.71|11616.62|12700|3268.02|4100
12|12. Customer Operations Expense|21|6623|CUSTOMER BILLING EXPENSE|4801.6|5145.6|5100|1708.8|1700
12|12. Customer Operations Expense|21|6623.45|CUSTOMER SERVICE-TECHNICAL SUPPO|10387.7|16912.22|32400|5477.34|8000
12|12. Customer Operations Expense|21|6623.55|CUSTOMER SERV-GENERAL ADMINISTRA|0|0|0|0|0
12|12. Customer Operations Expense|21|6623.9|COST OF SALES|0|0|0|0|0
12|12. Customer Operations Expense|22|6611.1|SALES COMMISSIONS EXPENSE|3921.99|10281.16|24800|0|8600
12|12. Customer Operations Expense|22|6612.1|SALES/MARKETING|18308.37|15293.19|73425|12013.19|18625
12|12. Customer Operations Expense|22|6613.1|PRODUCT ADVERTISING|20985.21|21367.39|41600|6501.2|7350
12|12. Customer Operations Expense|22|6623|CUSTOMER BILLING EXPENSE|812.8|1054.8|1600|357.6|600
12|12. Customer Operations Expense|22|6623.1|CUSTOMER SERV-COMM-REQUEST PROC|50.37|0|1000|0|100
12|12. Customer Operations Expense|22|6623.3|CUSTOMER SERV-TRS|8616.72|7140.51|7200|2380.17|2400
12|12. Customer Operations Expense|22|6623.45|CUSTOMER SERVICE-TECHNICAL SUPPO|62596.66|103302.14|198900|34646.19|56100
12|12. Customer Operations Expense|22|6623.5|CUSTOMER SERV-CARRIER ACCESS BILLI|1584.67|1051.58|2600|606.86|800
12|12. Customer Operations Expense|22|6623.55|CUSTOMER SERV-GENERAL ADMINISTRA|8665.61|17611.04|27800|6116.48|7800
12|12. Customer Operations Expense|22|6623.9|COST OF SALES|0|0|0|0|0
12|12. Customer Operations Expense|23|6613.1|PRODUCT ADVERTISING|0|0|0|0|0
12|12. Customer Operations Expense|23|6623|CUSTOMER BILLING EXPENSE|31380|24816.4|27000|8256.8|9000
12|12. Customer Operations Expense|23|6623.55|CUSTOMER SERV-GENERAL ADMINISTRA|196.23|38.24|400|20.62|100
12|12. Customer Operations Expense|25|6611.1|SALES COMMISSIONS EXPENSE|2281.58|7379.14|20300|0|5100
12|12. Customer Operations Expense|25|6612.1|SALES/MARKETING|25719.64|42956.37|104875|12492.27|21175
12|12. Customer Operations Expense|25|6613.1|PRODUCT ADVERTISING|14748.51|17758.59|24450|4798.81|9050
12|12. Customer Operations Expense|25|6623|CUSTOMER BILLING EXPENSE|908.4|887.6|1800|287.6|600
12|12. Customer Operations Expense|25|6623.1|CUSTOMER SERV-COMM-REQUEST PROC|270.3|0|600|0|600
12|12. Customer Operations Expense|25|6623.45|CUSTOMER SERVICE-TECHNICAL SUPPO|0|0|0|0|0
12|12. Customer Operations Expense|25|6623.55|CUSTOMER SERV-GENERAL ADMINISTRA|10140.72|10775.89|17400|3909.07|5000
12|12. Customer Operations Expense|25|6623.9|COST OF SALES|54935.95|60130.82|54000|25050.69|18000
12|12. Customer Operations Expense|26|6612.1|SALES/MARKETING|0|0|900|0|0
12|12. Customer Operations Expense|26|6613.1|PRODUCT ADVERTISING|8849.25|9122.87|12450|3079.93|2800
12|12. Customer Operations Expense|26|6613.11|WEBSITE EXPENSES|0|0|0|0|0
12|12. Customer Operations Expense|26|6623|CUSTOMER BILLING EXPENSE|206|228.8|300|81.6|100
12|12. Customer Operations Expense|26|6623.1|CUSTOMER SERV-COMM-REQUEST PROC|0|0|200|0|0
12|12. Customer Operations Expense|26|6623.42|CUSTOMER SERV-BILLING CONTRACT|0|0|0|0|0
12|12. Customer Operations Expense|26|6623.45|CUSTOMER SERVICE-TECHNICAL SUPPO|1192.68|848.34|2400|591.72|900
12|12. Customer Operations Expense|26|6623.5|CUSTOMER SERV-CARRIER ACCESS BILLI|0|0|0|0|0
12|12. Customer Operations Expense|26|6623.55|CUSTOMER SERV-GENERAL ADMINISTRA|4868.88|8721.43|12600|3505.06|4900
12|12. Customer Operations Expense|28|6612.1|SALES/MARKETING|1969.46|0|0|0|0
12|12. Customer Operations Expense|28|6613.1|PRODUCT ADVERTISING|0|0|0|0|0
12|12. Customer Operations Expense|28|6613.11|WEBSITE EXPENSES|0|0|0|0|0
12|12. Customer Operations Expense|28|6623|CUSTOMER BILLING EXPENSE|0|0|0|0|0
12|12. Customer Operations Expense|28|6623.1|CUSTOMER SERV-COMM-REQUEST PROC|0|0|0|0|0
12|12. Customer Operations Expense|28|6623.45|CUSTOMER SERVICE-TECHNICAL SUPPO|0|331.85|0|117.6|0
12|12. Customer Operations Expense|28|6623.55|CUSTOMER SERV-GENERAL ADMINISTRA|0|0|0|0|0
12|12. Customer Operations Expense|29|6612.1|SALES/MARKETING|0|0|0|0|0
12|12. Customer Operations Expense|29|6613.1|PRODUCT ADVERTISING|0|0|0|0|0
12|12. Customer Operations Expense|29|6613.11|WEBSITE EXPENSES|0|0|0|0|0
12|12. Customer Operations Expense|29|6623|CUSTOMER BILLING EXPENSE|0|0|0|0|0
12|12. Customer Operations Expense|29|6623.55|CUSTOMER SERV-GENERAL ADMINISTRA|0|0|0|0|0
12|12. Customer Operations Expense|29|6623.9|COST OF SALES|0|0|0|0|0
12|12. Customer Operations Expense|30|6622.1|DIRECTORY ADVERTISING EXPENSE|25829.11|27856.34|27500|8405.5|9300
12|12. Customer Operations Expense|30|6623.3|CUSTOMER SERV-TRS|0|0|0|0|0
12|12. Customer Operations Expense|50|6613.1|PRODUCT ADVERTISING|17.51|0|0|0|0
13|13. Corporate Operations Expense|10|6711.1|EXECUTIVE|129321.77|130517.68|154000|45303.09|48200
13|13. Corporate Operations Expense|10|6711.2|EXECUTIVE-DIRECTORS FEES & EXPENSE|95356.97|75286.1|102600|25457.11|31000
13|13. Corporate Operations Expense|10|6711.3|EXEC-DIRECTORS FEES & EXPENSE-AWA|53839.31|10075.25|59400|0|0
13|13. Corporate Operations Expense|10|6720.11|BANKING FEES|5796.5|4558.07|5400|1364.28|1900
13|13. Corporate Operations Expense|10|6721.1|ACCOUNTING & FINANCE-GENERAL|193456.97|175079.91|204100|64310.52|59900
13|13. Corporate Operations Expense|10|6721.2|ACCOUNTING & FINANCE-EXPENSE|14619.92|16076.06|15000|1467.56|5000
13|13. Corporate Operations Expense|10|6721.3|ACCOUNTING & FINANCE-COST SEPARA|1790.21|367.83|2100|0|0
13|13. Corporate Operations Expense|10|6721.35|ACCOUNTING & FINANCE-COST CONSUL|32468.46|8321|35000|0|10000
13|13. Corporate Operations Expense|10|6721.4|ACCOUNTING & FINANCE-AUDIT/PROF F|10000|22500|13000|0|0
13|13. Corporate Operations Expense|10|6722.1|EXTERNAL RELATIONS-CONNECT CO RE|10840.21|5194.07|14300|1296.87|800
13|13. Corporate Operations Expense|10|6723.1|HUMAN RESOURCES EXPENSE-BENEFITS|48148.53|53308.54|57300|21413.3|18900
13|13. Corporate Operations Expense|10|6723.2|HUMAN RESOURCES EXPENSE-LABOR RE|53781.91|43665.65|85600|14029.66|28800
13|13. Corporate Operations Expense|10|6724.1|INFORMATION MGT-DATA PROCESSING|3697.35|1249.97|1300|416.63|400
13|13. Corporate Operations Expense|10|6725|LEGAL EXPENSE|25682.13|34540.96|50900|11926|11900
13|13. Corporate Operations Expense|10|6728.1|OTHER GEN & ADMINISTRATIVE EXP-GE|541.44|389.76|1300|444.51|300
13|13. Corporate Operations Expense|10|6728.15|OTHER GEN & ADM EXP-OTHER|0|0|0|0|0
13|13. Corporate Operations Expense|10|6728.2|OTHER GEN & ADM EXP-SAFE/SEC|8620|174772.16|9200|-3474.46|1400
13|13. Corporate Operations Expense|10|6728.3|OTHER GEN & ADM EXP-INS PREM|52955.44|46298.01|36600|15481.67|12200
13|13. Corporate Operations Expense|10|6728.4|OTHER GEN & ADM EXP-SWITCHBOARD|20621.41|19406.88|17500|7898.94|5500
13|13. Corporate Operations Expense|10|6728.5|OTHER GEN & ADM EXP-ACCIDENT/DAM|317.29|779.36|3200|0|1100
13|13. Corporate Operations Expense|10|6728.55|OTHER GEN & ADM EXP-POSTAGE & DELI|3381.48|9178.34|5200|3121.68|1700
13|13. Corporate Operations Expense|10|6728.6|OTHER GEN & ADM EXP-ANNUAL MEETI|0|0|0|0|0
13|13. Corporate Operations Expense|10|6728.7|OTHER GEN & ADM EXP-ASSOCIATION D|29147.46|31055.67|34000|10351.89|11300
13|13. Corporate Operations Expense|10|6728.86|OTHER GEN & ADM EXP-REGULATORY F|20|20.95|100|20.95|0
13|13. Corporate Operations Expense|10|6728.9|MANAGEMENT FEE EXPENSE - TLC|0|0|0|0|0
13|13. Corporate Operations Expense|10|6728.91|MANAGEMENT FEE EXPENSE-FIBER|0|0|0|0|0
13|13. Corporate Operations Expense|10|6728.96|MANAGEMENT FEE EXPENSE -BEC|0|0|0|0|0
13|13. Corporate Operations Expense|10|6729|RELIEF, PENSIONS, ETC--CLEARING ACC|0|0|0|0|0
13|13. Corporate Operations Expense|20|6700|PAYROLL EXPENSE|0|0|0|0|0
13|13. Corporate Operations Expense|20|6711.1|EXECUTIVE|-17144.87|44|12600|0|500
13|13. Corporate Operations Expense|20|6711.2|EXEC-DIRECTORS FEES & EXPENSE-HOM|0|9600|0|3300|0
13|13. Corporate Operations Expense|20|6720.11|BANKING FEES|197.52|223.05|300|74.3|100
13|13. Corporate Operations Expense|20|6720.5|GEN & ADMIN-CONSULTING|0|0|0|0|0
13|13. Corporate Operations Expense|20|6721.1|ACCOUNTING & FINANCE|0|183.19|0|183.19|0
13|13. Corporate Operations Expense|20|6721.4|ACCOUNTING & FINANCE-AUDIT/PROF F|9500|13500|15000|8500|0
13|13. Corporate Operations Expense|20|6723.1|HUMAN RESOURCES EXPENSE-BENEFITS|0|1237.29|1600|353.22|500
13|13. Corporate Operations Expense|20|6723.2|HUMAN RESOURCES EXPENSE-LABOR RE|15513.3|3977.27|13200|950.22|1400
13|13. Corporate Operations Expense|20|6725|LEGAL EXPENSE|32853.74|3000|2300|1950|800
13|13. Corporate Operations Expense|20|6728.1|OTHER GEN & ADMINISTRATIVE EXP-GE|0|0|150|0|50
13|13. Corporate Operations Expense|20|6728.2|OTHER GEN & ADM EXP-SAFE/SEC|1135|1167.37|1100|-2968|100
13|13. Corporate Operations Expense|20|6728.3|OTHER GEN & ADM EXP-INS PREM|0|3258.46|0|1313.38|0
13|13. Corporate Operations Expense|20|6728.35|OTHER GEN & ADM EXP-EMPLOYEE BEN|2712.53|6800.41|4500|2375.4|1500
13|13. Corporate Operations Expense|20|6728.55|OTHER GEN & ADM EXP-POSTAGE & DELI|0|19.94|0|19.94|0
13|13. Corporate Operations Expense|20|6728.7|OTH GEN & ADM EXP-ASSOCIATION DUE|0|0|0|0|0
13|13. Corporate Operations Expense|20|6728.85|OTH GEN & ADM EXP-LICENSES & PERMI|0|0|0|0|0
13|13. Corporate Operations Expense|20|6728.86|OTH GEN & ADM EXP-REGULATORY FEE|40.47|0|0|0|0
13|13. Corporate Operations Expense|20|6728.9|MANAGEMENT FEE EXPENSE - TLC GEN|28721.9|39199.6|38100|14286.06|13100
13|13. Corporate Operations Expense|20|6729|RELIEF, PENSIONS, ETC--CLEARING ACC|0|0|0|0|0
13|13. Corporate Operations Expense|20|6760|TRAVEL EXPENSES|0|0|0|0|0
13|13. Corporate Operations Expense|21|6720.5|GEN & ADMIN-CONSULTING|0|0|0|0|0
13|13. Corporate Operations Expense|21|6721.2|ACCOUNTING & FINANCE-EXPENSE|0|0|300|0|100
13|13. Corporate Operations Expense|21|6725|LEGAL EXPENSE|0|2593|0|2593|0
13|13. Corporate Operations Expense|21|6728.1|OTHER GEN & ADMINISTRATIVE EXP-GE|0|0|0|0|0
13|13. Corporate Operations Expense|21|6728.75|OTH GEN & ADM EXP-DATA SERVICE EXP|0|0|0|0|0
13|13. Corporate Operations Expense|21|6728.86|OTH GEN & ADM EXP-REGULATORY FEE|5550|5250|5550|1750|1850
13|13. Corporate Operations Expense|21|6728.9|MANAGEMENT FEE EXPENSE - VIDEO/IPT|1565.07|4010.61|3600|1397.95|900
13|13. Corporate Operations Expense|22|6711.1|EXECUTIVE|0|0|16800|0|5800
13|13. Corporate Operations Expense|22|6720.5|GEN & ADMIN-CONSULTING|0|0|0|0|0
13|13. Corporate Operations Expense|22|6721.2|ACCOUNTING & FINANCE-EXPENSE|16.98|3415.12|5100|7.44|1700
13|13. Corporate Operations Expense|22|6721.4|ACCOUNTING & FINANCE-AUDIT/PROF F|0|0|1500|0|500
13|13. Corporate Operations Expense|22|6725|LEGAL EXPENSE|0|375|0|0|0
13|13. Corporate Operations Expense|22|6728.1|OTHER GEN & ADMINISTRATIVE EXP-GE|19.44|0|0|0|0
13|13. Corporate Operations Expense|22|6728.55|OTHER GEN & ADM EXP-POSTAGE & DELI|562.23|0|0|0|0
13|13. Corporate Operations Expense|22|6728.85|OTH GEN & ADM EXP-LICENSES & PERMI|0|0|0|0|0
13|13. Corporate Operations Expense|22|6728.86|OTH GEN & ADM EXP-REGULATORY FEE|1050|3000|1050|1000|350
13|13. Corporate Operations Expense|22|6728.9|MANAGEMENT FEE EXPENSE - TLC CLEC|3772.95|1986.61|2700|186.57|900
13|13. Corporate Operations Expense|23|6720|GENERAL AND ADMINISTRATIVE|0|0|0|0|0
13|13. Corporate Operations Expense|23|6725|LEGAL EXPENSE|0|0|0|0|0
13|13. Corporate Operations Expense|23|6728.1|OTHER GEN & ADMINISTRATIVE EXP-GE|0|0|0|0|0
13|13. Corporate Operations Expense|23|6728.86|OTH GEN & ADM EXP-REGULATORY FEE|0|0|0|0|0
13|13. Corporate Operations Expense|23|6728.9|MANAGEMENT FEE EXPENSE - LD|282.22|628.05|800|140.66|300
13|13. Corporate Operations Expense|25|6700|PAYROLL EXPENSE|0|0|0|0|0
13|13. Corporate Operations Expense|25|6720.5|GEN & ADMIN-CONSULTING|4287.44|0|0|0|0
13|13. Corporate Operations Expense|25|6723.1|HUMAN RESOURCES EXPENSE-BENEFITS|0|0|0|0|0
13|13. Corporate Operations Expense|25|6725|LEGAL EXPENSE|0|0|0|0|0
13|13. Corporate Operations Expense|25|6728.1|OTHER GEN & ADMINISTRATIVE EXP-GE|13984.67|0|300|0|100
13|13. Corporate Operations Expense|25|6728.3|OTHER GEN & ADM EXP-INS PREM|1177.44|1235.28|1350|411.76|450
13|13. Corporate Operations Expense|25|6728.55|OTHER GEN & ADM EXP-POSTAGE & DELI|103.2|152|100|0|0
13|13. Corporate Operations Expense|25|6728.7|OTH GEN & ADM EXP-ASSOCIATION DUE|0|0|0|0|0
13|13. Corporate Operations Expense|25|6728.85|OTH GEN & ADM EXP-LICENSES & PERMI|0|75|600|75|600
13|13. Corporate Operations Expense|25|6728.86|OTH GEN & ADM EXP-REGULATORY FEE|0|0|0|0|0
13|13. Corporate Operations Expense|25|6728.9|MANAGEMENT FEE EXPENSE - SECURITY|5016.75|4601.25|7300|1651.12|2200
13|13. Corporate Operations Expense|25|6750|TRAINING|0|0|0|0|0
13|13. Corporate Operations Expense|26|6711.1|EXECUTIVE|0|0|750|0|0
13|13. Corporate Operations Expense|26|6711.2|EXEC-DIRECTORS FEES & EXPENSE-HOM|0|0|0|0|0
13|13. Corporate Operations Expense|26|6720.5|GEN & ADMIN-CONSULTING|0|0|0|0|0
13|13. Corporate Operations Expense|26|6721.2|ACCOUNTING & FINANCE-EXPENSE|0|3810.74|0|920.4|0
13|13. Corporate Operations Expense|26|6725|LEGAL EXPENSE|0|525|2500|0|0
13|13. Corporate Operations Expense|26|6728.1|OTHER GEN & ADMINISTRATIVE EXP-GE|0|0|0|0|0
13|13. Corporate Operations Expense|26|6728.55|OTHER GEN & ADM EXP-POSTAGE & DELI|0|54.62|0|0|0
13|13. Corporate Operations Expense|26|6728.85|OTH GEN & ADM EXP-LICENSES & PERMI|0|0|0|0|0
13|13. Corporate Operations Expense|26|6728.9|MANAGEMENT FEE EXPENSE - BEC|2998.02|7887.65|5500|2578.18|1700
13|13. Corporate Operations Expense|26|6750|TRAINING EXPENSE|0|0|0|0|0
13|13. Corporate Operations Expense|27|6711.1|EXECUTIVE|0|0|0|0|0
13|13. Corporate Operations Expense|27|6721.4|ACCOUNTING & FINANCE-AUDIT/PROF F|0|0|0|0|0
13|13. Corporate Operations Expense|27|6725|LEGAL EXPENSE|0|0|0|0|0
13|13. Corporate Operations Expense|27|6728.9|MANAGEMENT FEE EXPENSE - VIBRANT|0|0|0|0|0
13|13. Corporate Operations Expense|28|6711.1|EXECUTIVE|31167.81|0|0|0|0
13|13. Corporate Operations Expense|28|6720.5|GEN & ADMIN-CONSULTING|0|0|0|0|0
13|13. Corporate Operations Expense|28|6721.1|ACCOUNTING & FINANCE-GENERAL|359.4|3056.61|0|1029.84|0
13|13. Corporate Operations Expense|28|6721.2|ACCOUNTING & FINANCE-EXPENSE|0|0|0|0|0
13|13. Corporate Operations Expense|28|6721.4|ACCOUNTING & FINANCE-AUDIT/PROF F|0|0|0|0|0
13|13. Corporate Operations Expense|28|6723.1|HUMAN RESOURCES EXPENSE-BENEFITS|0|0|0|0|0
13|13. Corporate Operations Expense|28|6725|LEGAL EXPENSE|3125|600|0|600|0
13|13. Corporate Operations Expense|28|6728.1|OTHER GEN & ADMINISTRATIVE EXP-GE|0|0|0|0|0
13|13. Corporate Operations Expense|28|6728.55|OTHER GEN & ADM EXP-POSTAGE & DELI|0|0|0|0|0
13|13. Corporate Operations Expense|28|6728.9|MANAGEMENT FEE EXPENSE - HAPPYBY|906.06|29.84|0|-2314.08|0
13|13. Corporate Operations Expense|29|6711.1|EXECUTIVE|0|0|0|0|0
13|13. Corporate Operations Expense|29|6711.2|EXEC-DIRECTORS FEES & EXPENSE-HOM|0|0|0|0|0
13|13. Corporate Operations Expense|29|6720.5|GEN & ADMIN-CONSULTING|0|0|0|0|0
13|13. Corporate Operations Expense|29|6721.4|ACCOUNTING & FINANCE-AUDIT/PROF F|0|0|0|0|0
13|13. Corporate Operations Expense|29|6725|LEGAL EXPENSE|0|0|0|0|0
13|13. Corporate Operations Expense|29|6728.1|OTHER GEN & ADMINISTRATIVE EXP-GE|0|0|0|0|0
13|13. Corporate Operations Expense|29|6728.86|OTH GEN & ADM EXP-REGULATORY FEE|0|0|0|0|0
13|13. Corporate Operations Expense|29|6728.9|MANAGEMENT FEE EXPENSE - TRCCA|0|0|0|0|0
13|13. Corporate Operations Expense|30|6711.1|EXECUTIVE|0|0|0|0|0
13|13. Corporate Operations Expense|30|6720.11|BANKING FEES|12|0|0|0|0
13|13. Corporate Operations Expense|30|6720.5|GEN & ADMIN-CONSULTING|0|0|0|0|0
13|13. Corporate Operations Expense|30|6721.2|ACCOUNTING & FINANCE-EXPENSE|0|0|150|0|50
13|13. Corporate Operations Expense|30|6721.4|ACCOUNTING & FINANCE-AUDIT/PROF F|7000|10500|8150|8000|50
13|13. Corporate Operations Expense|30|6725|LEGAL EXPENSE|375|0|5000|0|0
13|13. Corporate Operations Expense|30|6728.3|OTHER GEN & ADM EXP-INS PREM|0|1251.87|0|417.29|0
13|13. Corporate Operations Expense|30|6728.55|OTHER GEN & ADM EXP-POSTAGE & DELI|32|0|100|0|0
13|13. Corporate Operations Expense|30|6728.85|OTH GEN & ADM EXP-LICENSES & PERMI|0|0|0|0|0
13|13. Corporate Operations Expense|30|6728.86|OTH GEN & ADM EXP-REGULATORY FEE|755.53|0|800|0|800
13|13. Corporate Operations Expense|30|6728.91|MANAGEMENT FEE EXPENSE-FIBER|2771.64|4596.47|3000|1536.27|1200
13|13. Corporate Operations Expense|50|6711.1|EXECUTIVE|472.25|0|0|0|0
13|13. Corporate Operations Expense|50|6720.5|GEN & ADMIN-CONSULTING|6509.15|0|0|0|0
13|13. Corporate Operations Expense|50|6721.1|ACCOUNTING & FINANCE-GENERAL|1590.19|0|0|0|0
13|13. Corporate Operations Expense|50|6725|LEGAL EXPENSE|8132.5|0|0|0|0
13|13. Corporate Operations Expense|50|6728.1|OTHER GEN & ADMINISTRATIVE EXP-GE|57.39|0|0|0|0
13|13. Corporate Operations Expense|50|6728.86|OTH GEN & ADM EXP-REGULATORY FEE|20|0|0|0|0
17|17. State and Local Taxes|10|7370|GAIN OR LOSS ON INVESTMENTS|0|0|0|0|0
19|19. Other Taxes|10|7240.1|OTHER OPERATING TAXES-PROPERTY|165000|145500|145500|48500|48500
19|19. Other Taxes|10|7240.2|OTHER OPERATING TAXES-TN FRANCHIS|0|0|0|0|0
19|19. Other Taxes|10|7240.3|OTHER OPERATING TAXES-CLEARANCE|0|0|0|0|0
19|19. Other Taxes|20|7240.1|OTHER OPERATING TAXES-PROPERTY|18000|15140|14400|5540|4800
19|19. Other Taxes|20|7240.2|OTHER OPERATING TAXES-TN FRANCHIS|0|15000|0|0|0
19|19. Other Taxes|20|7240.3|OTHER OPERATING TAXES-CLEARANCE|0|0|0|0|0
19|19. Other Taxes|26|7240.1|OTHER OPERATING TAXES-PROPERTY|0|0|0|0|0
19|19. Other Taxes|30|7240.1|OTHER OPERATING TAXES-PROPERTY|2400|2100|2100|700|700
19|19. Other Taxes|30|7240.2|OTHER OPERATING TAXES-TN FRANCHIS|0|1800|1350|0|450
19|19. Other Taxes|30|7250|DEFERRED INCOME TAX EXPENSE|0|0|0|0|0
2|2. Network Access Services Revenue|10|5081.1|END USER ACCESS REV-R1-B1 -INTERSTA|533699.85|484207.9|478500|161156.59|158000
2|2. Network Access Services Revenue|10|5081.11|END USER ACCESS CHARGE-FCC LIFELIN|-21511.35|-18783.86|-12000|-6162.96|-4000
2|2. Network Access Services Revenue|10|5081.115|LIFELINE NECA-USAC TIER voice|16485|13175|12300|4398|4100
2|2. Network Access Services Revenue|10|5081.12|LIFELINE CREDIT-BROADBAND ONLY|-456.94|-859.33|-300|-291.07|-100
2|2. Network Access Services Revenue|10|5081.15|ACCESS RECOVERY CHARGE|7.8|0|0|0|0
2|2. Network Access Services Revenue|10|5081.16|ACCESS RECOVERY CHARGE-LIFELINE|0|0|0|0|0
2|2. Network Access Services Revenue|10|5081.17|ACCESS RECOVERY CHARGE MULTI LN B|34490|33106.7|33800|10972|10900
2|2. Network Access Services Revenue|10|5081.18|ACCESS RECOVERY CHARGE SINGL LN B|8761.7|8542.8|8400|2857.4|2800
2|2. Network Access Services Revenue|10|5081.4|END USER ACCESS-FUSC|115294.83|108960.77|141000|34847.72|47000
2|2. Network Access Services Revenue|10|5081.55|END USER ACCESS-LINE PORT CHARGE O|2662.9|2539.08|2400|846.36|800
2|2. Network Access Services Revenue|10|5082.1|SWTCHD ACC REV-T S INTERSTATE|37671.61|31570.8|27000|9580.06|9000
2|2. Network Access Services Revenue|10|5082.11|SWTCHD ACC REV-DIRECT TRUNK-INTER|8940.53|9092.61|9000|3030.87|3000
2|2. Network Access Services Revenue|10|5082.4|SWTCHD ACC REV-OTHER-INTERSTATE|1257.47|1294.57|1200|427.92|400
2|2. Network Access Services Revenue|10|5082.5|SWTCHD ACC REV-CCL POOL-NECA-INTR|292434|-832446|0|-304122|0
2|2. Network Access Services Revenue|10|5082.51|UNIVERSAL SERVICE FUND - HCL/CAF BL|1369064|1139586|948000|380456|316000
2|2. Network Access Services Revenue|10|5082.53|UNIVERSAL SERVICE FUND - ICLS|2443374|2290212|2298000|763404|766000
2|2. Network Access Services Revenue|10|5082.54|CONNECT AMERICA FUND (CAF)-ICC|75942|259131|258000|86377|86000
2|2. Network Access Services Revenue|10|5082.6|SWTCHD ACC REV-T S PL-NECA-INTRST|19628|-50|0|-45|0
2|2. Network Access Services Revenue|10|5082.81|SWITCHED ACCESS - PIC CHANGE|0|0|0|0|0
2|2. Network Access Services Revenue|10|5083.1|SPEC ACC REV-DATA CCT-INTERSTATE|171485.24|131105.83|156000|43058.85|52000
2|2. Network Access Services Revenue|10|5083.1001|SPEC ACCESS REV-DATA CCT-INTERSTE-|1079.43|1050.03|900|350.01|300
2|2. Network Access Services Revenue|10|5083.13|SPEC ACC REV-ETS-ETHERNET|101816.77|32435.66|36000|10708.82|12000
2|2. Network Access Services Revenue|10|5083.1301|SPEC ACC REV-ETS-NOT FUSC EXEMPT C|5349.36|5019.18|5100|1555.92|1700
2|2. Network Access Services Revenue|10|5083.4|SPEC ACC REV-BROADBAND|354626.8|424865.62|418500|141423.18|138000
2|2. Network Access Services Revenue|10|5083.42|SPEC ACC REV-CBOL|433566|887460|801600|312648|275600
2|2. Network Access Services Revenue|10|5083.45|SPEC ACC REV-MISC IRIS|70227.66|71232.24|73500|23744.08|24500
2|2. Network Access Services Revenue|10|5084.1|ST ACC REV-TFC SENSTV-INTRALATA PL|89.38|3.83|0|1.67|0
2|2. Network Access Services Revenue|10|5084.13|ST ACC REV-SPECIAL ACCESS-INTRALAT|47256.9|35856.9|33900|11952.3|11300
2|2. Network Access Services Revenue|10|5084.135|ST ACC INTRA REV-ETHERNET|9782.92|9187.38|9000|3062.46|3000
2|2. Network Access Services Revenue|10|5084.14|ST ACC REV-CELLULAR|0|0|0|0|0
2|2. Network Access Services Revenue|10|5084.2|ST ACC REV-TRAFFIC SENSTVE-INTERLA|16458.35|14045.63|12600|4260.7|4200
2|2. Network Access Services Revenue|10|5084.22|ST ACC REV-SPECIAL ACCESS-INTERLAT|2817.48|2732.52|2700|910.84|900
2|2. Network Access Services Revenue|10|5084.23|ST ACC REV-OTHER-INTERLATA|130.86|104.46|0|33.76|0
2|2. Network Access Services Revenue|10|5084.24|ST ACC REV-SPEC ACCESS SURGE-INTER|0.59|0|0|0|0
2|2. Network Access Services Revenue|22|5081.11|END USER ACCESS CHARGE-FCC LIFELIN|0|0|0|0|0
2|2. Network Access Services Revenue|22|5081.12|LIFELINE CREDIT-CLEC BROADBAND ON|0|0|0|0|0
2|2. Network Access Services Revenue|22|5081.4|END USER ACCESS-FUSC|27340.77|15346.32|36000|1967.41|12000
2|2. Network Access Services Revenue|22|5081.55|END USER ACCESS-LINE PORT CHARGE|399.67|352.65|300|117.55|100
2|2. Network Access Services Revenue|22|5082.1|SWTCHD ACC REV-T S INTERSTATE|1609.51|1338.42|1200|355.47|400
2|2. Network Access Services Revenue|22|5082.11|SWTCHD ACC REV-DIRECT TRUNK-INTER|0|0|0|0|0
2|2. Network Access Services Revenue|22|5082.4|SWTCHD ACC REV-OTHER-INTERSTATE|53.23|46.42|0|14.15|0
2|2. Network Access Services Revenue|22|5083.1|SPEC ACC REV-DATA CCT-INTERSTATE|0|0|0|0|0
2|2. Network Access Services Revenue|22|5083.13|SPEC ACC REV-ETS-ETHERNET|372815.48|402360.03|405000|132878.35|136000
2|2. Network Access Services Revenue|22|5083.1301|SPEC ACC REV-ETS-NOT FUSC EXEMPT C|2247|1365.9|2100|-1734.1|700
2|2. Network Access Services Revenue|22|5084.13|ST ACC REV-SPECIAL ACCESS-INTRALAT|0|0|0|0|0
2|2. Network Access Services Revenue|22|5084.2|ST ACC REV-TRAFFIC SENSTVE-INTERLA|0|0|0|0|0
2|2. Network Access Services Revenue|22|5084.21|ST ACC REV-CCL-INTERLATA|0|0|0|0|0
2|2. Network Access Services Revenue|22|5084.22|ST ACC REV-SPECIAL ACCESS-INTERLAT|31847.87|90473.39|49500|32067.61|17000
2|2. Network Access Services Revenue|22|5084.23|ST ACC REV-OTHER-INTERLATA|0|0|0|0|0
2|2. Network Access Services Revenue|23|5082.81|SWITCHED ACCESS - PIC CHANGE|13.75|22|0|8.25|0
2|2. Network Access Services Revenue|30|5081|END USER ACCESS REVENUE|0|0|0|0|0
2|2. Network Access Services Revenue|30|5081.4|END USER ACCESS-FUSC|0|0|0|0|0
2|2. Network Access Services Revenue|30|5083.13|SPEC ACC REV-ETS-ETHERNET|7500|7500|7500|2500|2500
22|22. Interest on Funded Debt|10|7510|INTEREST ON FUNDED DEBT - RUS|286295.5|281965.2|292000|94328.16|98000
22|22. Interest on Funded Debt|10|7510.1|INTEREST ON FUNDED DEBT - RUS STIMU|94031.25|82126.68|85000|34881.29|34000
22|22. Interest on Funded Debt|20|7510.11|INTEREST ON FUNDED DEBT - COBANK|38461.26|25974.69|40000|3174.69|13000
22|22. Interest on Funded Debt|20|7510.2|INTEREST ON FUNDED DEBT-FNBOT BLD|1735.83|0|0|0|0
22|22. Interest on Funded Debt|20|7511.1|FINANCE CHARGE-COBANK COMMITME|1479.63|2140.17|3900|0|1300
22|22. Interest on Funded Debt|25|7510.2|INTEREST ON FUNDED DEBT-FNBOT BLD|0|0|0|0|0
24|24. Other Interest Expense|10|7540|OTHER INTEREST EXPENSES|247.88|346.79|0|119.85|0
25|25. Allowance For Funds Used During Construction|10|7340|ALLOWANCE-FUNDS DURING CONST|652.91|0|0|0|0
27|27. Nonoperating Net Income|10|7310.2|DIVIDEND INCOME|0|0|0|0|0
27|27. Nonoperating Net Income|10|7320|INTEREST INCOME|235175.22|187069.59|145000|57012.88|47000
27|27. Nonoperating Net Income|10|7361|OTHER NONOP INCOME-INVESTMENTS|22423.88|10241.29|0|0|0
27|27. Nonoperating Net Income|10|7361.1|INCOME FROM SUBSIDIARY-TRULINK|496370.74|319975.12|-38800|0|37400
27|27. Nonoperating Net Income|10|7361.2|INCOME FROM SUBSIDIARY-FIBER|17967.48|4979.88|-2000|0|2525
27|27. Nonoperating Net Income|10|7380|CHARITABLE CONTRIBUTIONS|-750|-12904.65|-53300|0|-10100
27|27. Nonoperating Net Income|10|7390|OTHER NONOPERATING EXPENSE|0|0|0|0|0
27|27. Nonoperating Net Income|10|7440|TN EXCISE TAX EXP|0|0|0|0|0
27|27. Nonoperating Net Income|10|8100.13|ETHERNET CONNECTION TO ISP|8547.33|7290|7200|2430|2400
27|27. Nonoperating Net Income|10|8100.15|WEB HOSTING|567.89|669.82|600|230.16|200
27|27. Nonoperating Net Income|10|8100.16|DATA BACKUP|3242.97|3242.97|3000|1080.99|1000
27|27. Nonoperating Net Income|10|8100.18|BROADBAND EARLY TERM FEE|4900|0|0|0|0
27|27. Nonoperating Net Income|10|8100.3|BROADBAND REV-RESIDENTIAL|2336435.48|2483046.63|2407500|840759.51|796000
27|27. Nonoperating Net Income|10|8100.31|BROADBAND INSTALL REV|85|0|0|0|0
27|27. Nonoperating Net Income|10|8100.32|BROADBAND REV-MODEM/ROUTER WOR|199108.38|246402.23|243000|84183.1|82000
27|27. Nonoperating Net Income|10|8100.35|BROADBAND PROMO EXP- MODEMS/EQU|-100089.35|-159627.63|-132500|-44491.77|-40000
27|27. Nonoperating Net Income|10|8100.4|BROADBAND REV-BUSINESS|359783.99|366278.64|346500|124535.84|116000
27|27. Nonoperating Net Income|10|8100.41|BUNDLE CREDIT-BROADBAND|-73601.14|-335581.71|-309000|-117877.79|-103000
27|27. Nonoperating Net Income|10|8100.42|2-SERVICE BUNDLE CREDIT|0|0|0|0|0
27|27. Nonoperating Net Income|10|8100.43|3-SERVICE BUNDLE CREDIT|0|0|0|0|0
27|27. Nonoperating Net Income|10|8100.44|4-SERVICE BUNDLE CREDIT|0|0|0|0|0
27|27. Nonoperating Net Income|10|8100.45|PROMO BILLING CR & BUNDLE CREDIT|-89412.25|-72748.85|-69000|-30296.25|-23000
27|27. Nonoperating Net Income|10|8100.5|STATIC IP CHARGE|43404.34|111049.01|81000|53167.65|27000
27|27. Nonoperating Net Income|10|8100.51|MISC REV-INTERNET-USAGE CHARGE|16313.44|10637.45|10950|3361.33|3500
27|27. Nonoperating Net Income|10|8100.6|NONREG EXP-BB/INTERNET EXPENSES|-201117.51|-395163.78|-357000|-115270.14|-120500
27|27. Nonoperating Net Income|10|8100.61|NONREG EXP-BB/INTERNET-LEASED|-20|0|0|0|0
27|27. Nonoperating Net Income|10|8100.65|NONREG EXP-BB/INTERNET-CUST SERV|-76696.32|-120443.15|-79100|-41971.28|-25800
27|27. Nonoperating Net Income|10|8100.7|NONREG EXP-BB/INTERNET INSTALLATI|-17606.55|-18464.26|-14100|-4851.49|-2700
27|27. Nonoperating Net Income|10|8100.8|NONREG EXP-BB/INTERNET-LEASED CIR|-318010.41|-362566.84|-336000|-120569.18|-110000
27|27. Nonoperating Net Income|10|8100.82|NONREG EXP-CBOL|-433566|-887460|-891000|-312648|-309000
27|27. Nonoperating Net Income|10|8100.84|MODEM/EQUIPMENT SALES|5099.05|5136.69|4800|2255.93|1600
27|27. Nonoperating Net Income|10|8100.86|NONREG-BB/INTERNET EQUIP DEPR|-8827.64|-14298.48|-10500|0|-3500
27|27. Nonoperating Net Income|10|8100.88|NONREG-WIRELESS INTENET-DEPR EXP|-10209.86|-1119.17|-25200|0|-8400
27|27. Nonoperating Net Income|10|8110.21|WILDBLUE INTERNET EXPENSES|0|31.72|0|31.72|0
27|27. Nonoperating Net Income|10|8111.1|WIRELESS REVENUE|403.05|69.99|0|23.33|0
27|27. Nonoperating Net Income|10|8111.11|WIRELESS INTERNET INSTALL REV|480|850|0|35|0
27|27. Nonoperating Net Income|10|8111.2|WIRELESS EQUIPMENT MAINTENANCE|0|0|0|0|0
27|27. Nonoperating Net Income|10|8111.21|WIRELESS INTERNET EXPENSES|-10281|-10281|0|-3427|0
27|27. Nonoperating Net Income|10|8111.25|WIRELESS INTERNET LABOR EXP|-499.77|0|0|0|0
27|27. Nonoperating Net Income|10|8111.32|WIRELESS INC-WORRY FREE|474.59|1723.26|0|557.48|0
27|27. Nonoperating Net Income|10|8111.35|WIRELESS EQUIPMENT TRADE-IN|-1653.21|-2106.98|-1500|-603.18|-500
27|27. Nonoperating Net Income|10|8111.84|WIRELESS EQUIP SALES -ROUTER/EXTEN|820.75|116.04|0|99.99|0
27|27. Nonoperating Net Income|10|8120.3|BROADBAND ONLY- RESIDENTIAL|749644.16|1429237.18|1374000|507851.42|474000
27|27. Nonoperating Net Income|10|8120.4|BROADBAND ONLY- BUSINESS|15067.72|32952.08|36900|12139.79|12900
27|27. Nonoperating Net Income|20|7310.2|DIVIDEND INCOME|31359.5|0|25000|0|0
27|27. Nonoperating Net Income|20|7320|INTEREST INCOME|99.97|96.16|100|32.05|0
27|27. Nonoperating Net Income|20|7361.1|INCOME FROM RELATED CO-UBB|-6267.1|-58437.11|-242100|0|-80700
27|27. Nonoperating Net Income|20|7370|GAIN OR LOSS ON INVESTMENTS|0|772960.8|0|772960.8|0
27|27. Nonoperating Net Income|20|7380|CHARITABLE CONTRIBUTIONS|0|-1089|-6100|0|-2100
27|27. Nonoperating Net Income|20|8100.86|NONREG-BB/INTERNET EQUIP DEPR|0|0|0|0|0
27|27. Nonoperating Net Income|21|8200.01|IPTV PROMOTIONS EXPENSE|-6826.54|-8043.47|0|-1732.8|0
27|27. Nonoperating Net Income|21|8200.02|BUNDLE CREDIT-TV|0|-24620.49|0|-8224.52|0
27|27. Nonoperating Net Income|21|8200.1|VIDEO CONTENT EXPENSE|-623861.65|-688295.81|-691800|-229543.2|-230600
27|27. Nonoperating Net Income|21|8200.11|VIDEO REVENUE-PAY PER VIEW REV|342.68|539.56|300|84.91|100
27|27. Nonoperating Net Income|21|8200.12|VIDEO REVENUE-PREMIUM CHANNEL RE|27823.5|27625.56|27000|9250.52|9000
27|27. Nonoperating Net Income|21|8200.13|VIDEO INSTALLATION REVENUE|180|103|0|1|0
27|27. Nonoperating Net Income|21|8200.14|IPTV/VIDEO LEASED EQUIP-SET TOP BOX/|101980.01|109260.28|102000|34864.48|34000
27|27. Nonoperating Net Income|21|8200.15|VIDEO REVENUE-BASIC/EXPANDED BASI|907935.59|983556.72|960000|328737.7|320000
27|27. Nonoperating Net Income|21|8200.16|VIDEO REVENUE-HG TV UPGRADE/DOWN|240|140|300|20|100
27|27. Nonoperating Net Income|21|8200.17|VIDEO REVENUE-HIGH DEFINITION|54413.34|56738.56|54000|19006.74|18000
27|27. Nonoperating Net Income|21|8200.18|VIDEO REVENUE-EARLY TERMINATION F|8736.17|5052.05|7500|1125|2500
27|27. Nonoperating Net Income|21|8200.19|VIDEO REVENUE-RECONNECTION FEE|325|375|300|75|100
27|27. Nonoperating Net Income|21|8200.2|VIDEO CONTENT EXP-RETRANSMISSION|-183235.63|-216069.65|-209100|-72257.72|-69700
27|27. Nonoperating Net Income|21|8200.21|VIDEO CONTENT EXP-PAY PER VIEW|-446.75|-423.4|-300|32.44|-100
27|27. Nonoperating Net Income|21|8200.22|VIDEO CONTENT EXP-PREMIUM CHANNE|-23268.51|-23375.05|-24750|-7776.94|-8250
27|27. Nonoperating Net Income|21|8200.24|VIDEO MAINTENANCE EXPENSE|-64394.42|-109069.42|-84700|-24083.17|-27800
27|27. Nonoperating Net Income|21|8200.25|IPTV/VIDEO EXPENSE|0|-9509.25|0|-3180|0
27|27. Nonoperating Net Income|21|8200.26|IPTV/VIDEO HEAD END FEE|-19652.4|0|-4500|0|-1500
27|27. Nonoperating Net Income|21|8200.27|IPTV/VIDEO CUSTOMER SUPPORT EXPEN|-14810.16|-2132.59|-22800|-818.47|-7600
27|27. Nonoperating Net Income|21|8200.4|VIDEO REVENUE-WORRY FREE PROGRA|37973.95|40639.12|39000|13609.55|13000
27|27. Nonoperating Net Income|21|8200.41|VIDEO REVENUE-IPTV CALLER ID|403|348|300|114|100
27|27. Nonoperating Net Income|21|8200.5|VIDEO FRANCHISE FEE|47403.97|0|0|0|0
27|27. Nonoperating Net Income|21|8200.51|VIDEO RETRANSMISSION FEE REVENUE|151198.55|185459.09|171000|62020.22|57000
27|27. Nonoperating Net Income|21|8200.52|VIDEO-SPORTS FEE REVENUE|1587.9|1238.94|1500|403.98|500
27|27. Nonoperating Net Income|21|8200.9|COMMISSION INCOME|2367.04|2154.94|1200|964.39|400
27|27. Nonoperating Net Income|22|8100.13|ETHERNET CONNECTION TO ISP|6450|4000|4200|1200|1400
27|27. Nonoperating Net Income|22|8100.14|NETWORK MANAGEMENT|9773.41|6616.41|6600|2205.47|2200
27|27. Nonoperating Net Income|22|8100.18|BROADBAND- EARLY TERM FEE|125|0|0|0|0
27|27. Nonoperating Net Income|22|8100.3|BROADBAND REV- CLEC RESIDENTIAL|6433.74|12117.57|11700|3950.18|4000
27|27. Nonoperating Net Income|22|8100.31|BROADBAND INSTALL REV|530|75|300|75|100
27|27. Nonoperating Net Income|22|8100.32|BROADBAND REV-MODEM/ROUTER WOR|4732.82|7130.49|6500|2377.1|2200
27|27. Nonoperating Net Income|22|8100.35|BROADBAND PROMO EXP- MODEMS/EQU|-3625.73|-4896|-4900|-1464.81|-1700
27|27. Nonoperating Net Income|22|8100.4|BROADBAND REV-CLEC BUSINESS|200129.36|217179.3|211350|72912.03|70600
27|27. Nonoperating Net Income|22|8100.41|BUNDLE CREDIT-BROADBAND|-35179.01|-35744.51|-38100|-12079.74|-12800
27|27. Nonoperating Net Income|22|8100.42|2-SERVICE BUNDLE CREDIT|0|0|0|0|0
27|27. Nonoperating Net Income|22|8100.43|3-SERVICE BUNDLE CREDIT|0|0|0|0|0
27|27. Nonoperating Net Income|22|8100.44|4-SERVICE BUNDLE CREDIT|0|0|0|0|0
27|27. Nonoperating Net Income|22|8100.45|PROMO BILLING CR|-1215|-1200|-1200|-520|-400
27|27. Nonoperating Net Income|22|8100.5|STATIC IP CHARGE|76792.65|16452|15000|5587.16|5000
27|27. Nonoperating Net Income|22|8100.6|NONREG EXP-BB/INTERNET EXPENSES|0|-2257.48|-5100|-1992.96|-1800
27|27. Nonoperating Net Income|22|8100.65|NONREG EXP-BB/INTERNET-CUST SERV|-6072.4|-3919.15|-12000|-1178.75|-3500
27|27. Nonoperating Net Income|22|8100.7|NONREG EXP-INTERNET INSTALLATION-|0|0|0|0|0
27|27. Nonoperating Net Income|22|8100.75|NONREG EXP-BB/INTERNET MAINT LABO|-6137.97|-6337.97|-6600|-2117.99|-2200
27|27. Nonoperating Net Income|22|8100.84|MODEM/EQUIPMENT SALES|440.96|440.96|300|246.98|100
27|27. Nonoperating Net Income|22|8111.1|WIRELESS INTERNET REVENUE|724.61|0|0|0|0
27|27. Nonoperating Net Income|22|8111.11|WIRELESS INTERNET INSTALL REV|1270|150|1500|0|500
27|27. Nonoperating Net Income|22|8111.25|WIRELESS INTERNET LABOR EXP|-1619.68|-406.76|0|0|0
27|27. Nonoperating Net Income|22|8111.32|WIRELESS INC-WORRY FREE|1402.25|2119.96|1800|842.42|600
27|27. Nonoperating Net Income|22|8111.35|WIRELESS PROMO EXPENSE EXTENDER/E|0|-83.94|0|0|0
27|27. Nonoperating Net Income|22|8111.6|WIRELESS INTERNET COST OF GOODS SO|-2495.09|-3432.28|-3000|-97.57|-1000
27|27. Nonoperating Net Income|22|8111.84|WIRELESS EQUIP SALES -ROUTER/EXTEN|4543.54|199.99|3600|0|1200
27|27. Nonoperating Net Income|22|8120.3|BROADBAND ONLY- CLEC RESIDENTIAL|10199.43|20022.55|20700|7284.09|7200
27|27. Nonoperating Net Income|22|8120.4|BROADBAND ONLY REVENUE- CLEC BUS|49493.82|54393.23|52200|19281.32|17600
27|27. Nonoperating Net Income|25|5270.9|SALES INCOME EQUIPMENT/INVENTORY|-659.49|9488.83|15000|3854.46|5000
27|27. Nonoperating Net Income|25|8300.1|SECURITY MONITORING-BASIC|140478.25|149886.75|143000|48148.81|45000
27|27. Nonoperating Net Income|25|8300.11|SECURITY MONITORING-FIRE|9504.4|11095.5|9800|3569.4|3300
27|27. Nonoperating Net Income|25|8300.13|SECURITY MONITORING-CELLULAR|224.25|224.25|300|44.85|100
27|27. Nonoperating Net Income|25|8300.14|SECURITY MONITORING-ADDED FEATUR|5409.34|5254|5600|1483|1400
27|27. Nonoperating Net Income|25|8300.2|SECURITY VIDEO SERVICES|1098.67|3925.26|1300|1437.94|600
27|27. Nonoperating Net Income|25|8300.3|SECURITY SMART HOME SERVICES-ALA|12895.71|21478.08|50800|7343.46|4500
27|27. Nonoperating Net Income|25|8300.31|SECURITY SMART HOME SERVICES-MEDI|984.7|1062.04|1000|349.3|300
27|27. Nonoperating Net Income|25|8300.4|SECURITY WORRY FREE SERVICE|1221.36|1922.17|1200|671.06|400
27|27. Nonoperating Net Income|25|8300.9|SALES PROMO DISCOUNTS|-1472.1|-1313.05|-1800|-357.08|-600
27|27. Nonoperating Net Income|25|8310|SECURITY EQUP INSTALL REVENUE|63410.57|90438.9|90000|33126.11|30000
27|27. Nonoperating Net Income|25|8320|SECURITY SERVICE CALL REVENUE|6308.81|5949.98|7500|2000|2500
27|27. Nonoperating Net Income|26|8100.18|BROADBAND EARLY TERM FEE|25|0|0|0|0
27|27. Nonoperating Net Income|26|8100.3|BROADBAND REV- BEC RESIDENTIAL|6555.21|9075.06|6750|3495.85|2300
27|27. Nonoperating Net Income|26|8100.31|BEC BROADBAND INSTALL REV|0|0|0|0|0
27|27. Nonoperating Net Income|26|8100.32|BROADBAND REV- MODEM/ROUTER WO|1881.78|2453.97|2800|931.53|1000
27|27. Nonoperating Net Income|26|8100.35|BROADBAND PROMO EXP- MODEMS/EQU|-2673.38|-5945.41|-7500|-2911.89|-2500
27|27. Nonoperating Net Income|26|8100.4|BROADBAND REV- BEC BUSINESS|84.36|1319.97|1500|439.99|500
27|27. Nonoperating Net Income|26|8100.41|BUNDLE CREDIT-BROADBAND|-60|-1341.6|-900|-491.2|-300
27|27. Nonoperating Net Income|26|8100.42|2-SERVICE BUNDLE CREDIT|0|0|0|0|0
27|27. Nonoperating Net Income|26|8100.43|3-SERVICE BUNDLE CREDIT|0|0|0|0|0
27|27. Nonoperating Net Income|26|8100.44|4-SERVICE BUNDLE CREDIT|0|0|0|0|0
27|27. Nonoperating Net Income|26|8100.45|PROMO BILLING CR|-1610|-635|-900|-430|-300
27|27. Nonoperating Net Income|26|8100.5|STATIC IP CHARGE|10.81|104.55|0|37.95|0
27|27. Nonoperating Net Income|26|8100.6|NONREG EXP-BB/INTERNET EXPENSES|-10200.6|-12934.96|-19700|-4608.01|-7300
27|27. Nonoperating Net Income|26|8100.65|NONREG EXP-BB/INTERNET-CUST SERV|0|0|-1500|0|-700
27|27. Nonoperating Net Income|26|8100.7|NONREG EXP-INTERNET INSTALLATION-|0|0|0|0|0
27|27. Nonoperating Net Income|26|8100.75|NONREG EXP-BB/INTERNET MAINT-LABO|-22.97|-32.97|-300|-11.99|-100
27|27. Nonoperating Net Income|26|8100.84|MODEM/EQUIPMENT SALES|0|0|0|0|0
27|27. Nonoperating Net Income|26|8111.35|WIRELESS PROMO EXPENSE EXTENDER/E|0|0|0|0|0
27|27. Nonoperating Net Income|26|8120.3|BROADBAND ONLY- BEC RESIDENTIAL|19427.92|26322.42|25200|9655.72|8700
27|27. Nonoperating Net Income|26|8120.4|BROADBAND ONLY- BEC BUSINESS|2999.95|1079.97|1200|359.99|400
27|27. Nonoperating Net Income|26|8200.01|IPTV PROMOTIONS EXPENSE|-303|0|-15000|0|-5000
27|27. Nonoperating Net Income|26|8200.02|BUNDLE CREDIT-TV|0|-105.3|0|-35.1|0
27|27. Nonoperating Net Income|26|8200.1|VIDEO CONTENT PROGRAMMING/LICENS|-5939.41|-8013.66|-9450|-2714.92|-3150
27|27. Nonoperating Net Income|26|8200.11|VIDEO REVENUE-PAY PER VIEW REV|54.99|79.95|0|0|0
27|27. Nonoperating Net Income|26|8200.12|VIDEO REVENUE-PREMIUM CHANNEL PA|209.92|221.91|300|73.97|100
27|27. Nonoperating Net Income|26|8200.13|VIDEO INSTALLATION REVENUE|0|0|0|0|0
27|27. Nonoperating Net Income|26|8200.14|IPTV/VIDEO LEASED EQUIP-SET TOP BOX/|1700.38|1181.07|3000|401.33|1000
27|27. Nonoperating Net Income|26|8200.15|VIDEO REVENUE-EXPANDED BASIC|8111.63|11485.08|11600|3944.34|4000
27|27. Nonoperating Net Income|26|8200.16|VIDEO REVENUE-HG TV UPGRADE/DOWN|0|0|0|0|0
27|27. Nonoperating Net Income|26|8200.17|VIDEO REVENUE-HIGH DEFINITION|627.21|875.94|900|296.64|300
27|27. Nonoperating Net Income|26|8200.18|VIDEO REVENUE-EARLY TERMINATION F|250|0|0|0|0
27|27. Nonoperating Net Income|26|8200.19|VIDEO REVENUE-RECONNECTION FEE|0|0|0|0|0
27|27. Nonoperating Net Income|26|8200.2|VIDEO EXPENSE-RETRANSMISSION FEES|-1440.62|-2063.55|-3000|-338.36|-1000
27|27. Nonoperating Net Income|26|8200.22|VIDEO EXPENSE-PREMIUM CHANNEL PA|-189.37|-215.18|-450|-438.84|-150
27|27. Nonoperating Net Income|26|8200.24|VIDEO MAINTENANCE EXPENSE|0|0|-800|0|-400
27|27. Nonoperating Net Income|26|8200.25|IPTV/VIDEO EXPENSE|0|-100.5|0|-34.5|0
27|27. Nonoperating Net Income|26|8200.27|IPTV/VIDEO CUSTOMER SUPPORT EXPEN|0|-23.43|-150|-9.2|-50
27|27. Nonoperating Net Income|26|8200.4|VIDEO REVENUE-WORRY FREE PROGRA|213.12|356.91|300|129.94|100
27|27. Nonoperating Net Income|26|8200.41|VIDEO REVENUE-IPTV CALLER ID|3|0|0|0|0
27|27. Nonoperating Net Income|26|8200.5|VIDEO FRANCHISE FEE|0|0|0|0|0
27|27. Nonoperating Net Income|26|8200.51|VIDEO RETRANSMISSION FEE REVENUE|1210.99|1958.6|1800|671.52|600
27|27. Nonoperating Net Income|26|8200.52|VIDEO-SPORTS FEE REVENUE|0|0|0|0|0
27|27. Nonoperating Net Income|26|8200.9|COMMISSION INCOME|0|0|0|0|0
27|27. Nonoperating Net Income|28|8100.65|NONREG EXP-BB/INTERNET-CUST SERV|0|0|0|0|0
27|27. Nonoperating Net Income|28|8100.7|NONREG EXP-INTERNET INSTALLATION-|0|0|0|0|0
27|27. Nonoperating Net Income|28|8100.75|NONREG EXP-BB/INTERNET MAINT-LABO|0|0|0|0|0
27|27. Nonoperating Net Income|29|8100.75|NONREG EXP-BB/INTERNET MAINT-LABO|0|0|0|0|0
27|27. Nonoperating Net Income|30|7320|INTEREST INCOME|180|180.49|200|0|0
27|27. Nonoperating Net Income|30|7370|GAIN OR LOSS ON INVESTMENTS|0|0|0|0|0
27|27. Nonoperating Net Income|30|7400|FEDERAL TAX EXPENSE|-5000|-3100|-3450|0|-1150
27|27. Nonoperating Net Income|30|7410|LOCAL TAXES|0|0|0|0|0
27|27. Nonoperating Net Income|30|7420|OTHER TAXES|-410|-364|0|0|0
27|27. Nonoperating Net Income|30|7440|TN EXCISE TAX EXP|-3000|-300|-1200|0|-400
3|3. Long Distance Network Services Revenues|23|5100.52|LONG DIST REVENUE-INTERSTATE|143585.41|99955.22|135000|33716.37|45000
3|3. Long Distance Network Services Revenues|23|5100.53|LONG DIST REVENUE-INTRASTATE|112237.25|163355.65|105000|50295.97|35000
3|3. Long Distance Network Services Revenues|23|5100.54|LONG DIST REVENUE-INTERNATIONAL|-4270.7|2127.06|600|777.17|200
3|3. Long Distance Network Services Revenues|23|5100.55|LONG DIST REVENUE-CALL PLANS|0|0|0|0|0
3|3. Long Distance Network Services Revenues|26|5100.52|LONG DIST REVENUE-INTERSTATE|3383.3|4721.42|3000|1409.18|1000
3|3. Long Distance Network Services Revenues|26|5100.53|LONG DIST REVENUE-INTRASTATE|1821.79|161.56|1500|67.06|500
3|3. Long Distance Network Services Revenues|26|5100.54|LONG DIST REVENUE-INTERNATIONAL|0|0|0|0|0
3|3. Long Distance Network Services Revenues|26|5100.55|LONG DIST REVENUE-CALL PLANS|0|0|0|0|0
3|3. Long Distance Network Services Revenues|30|5100|LONG DIST REVENUE-FLAT RATE|0|0|0|0|0
30|30. Nonregulated Net Income|10|7990.02|NONREG SALES-PAGING/INTERCOM SYST|2226.17|2107.5|2100|709.65|700
30|30. Nonregulated Net Income|10|7990.03|NONREG SALES-KEY SYSTEMS|37416.53|0|0|0|0
30|30. Nonregulated Net Income|10|7990.04|NONREG SALES-PBX|11922.6|25795.56|24000|6161.3|8000
30|30. Nonregulated Net Income|10|7990.05|NONREG SALES-OTHER|10454.35|1844.37|6000|604.35|2000
30|30. Nonregulated Net Income|10|7990.07|NONREG SALES-HOSTED PBX|0|0|0|0|0
30|30. Nonregulated Net Income|10|7990.1|NONREG SALES-RES EQUIP|37730.83|34710.21|33000|11551.3|11000
30|30. Nonregulated Net Income|10|7990.11|NONREG SALES-BUS EQUIP|7577.28|15774.28|6000|11375.97|2000
30|30. Nonregulated Net Income|10|7990.13|WIRE MAINTENANCE- DATA SYSTEM|109252.19|117041.13|115350|40015.76|38600
30|30. Nonregulated Net Income|10|7990.14|NONREG INC-WIRE MAINT KEY SYSTEMS|22596.03|21995.9|21000|7354.38|7000
30|30. Nonregulated Net Income|10|7990.15|NONREG INC-WIRE MAINT PBX|97.5|68.25|0|22.75|0
30|30. Nonregulated Net Income|10|7990.21|NONREG INC-LEASED CONN CHARGES|-1257.31|57|0|0|0
30|30. Nonregulated Net Income|10|7990.22|NONREG INC-INSIDE WIRING R1-B1 CHGS|34302.28|33358.4|33000|12405.78|11000
30|30. Nonregulated Net Income|10|7990.24|NONREG EXP-INSTALL R1 & B1 PHONES|0|-155.75|0|0|0
30|30. Nonregulated Net Income|10|7990.25|NONREG EXP-MAINT R1 & B1 PHONES BU|0|2050|0|2050|0
30|30. Nonregulated Net Income|10|7990.26|NONREG EXP-INSTALL KEY SYSTEMS|-2503.09|-3569.87|0|-2447.65|0
30|30. Nonregulated Net Income|10|7990.27|NONREG EXP-INSTALLATION-PBX|-2789.02|0|0|0|0
30|30. Nonregulated Net Income|10|7990.28|NONREG EXP-MAINT R1 & B1 TELEPHON|-2420.46|-3863.34|-2400|-2045.88|-1200
30|30. Nonregulated Net Income|10|7990.29|NONREG EXP-MAINTENANCE-DATA EQUI|0|0|0|0|0
30|30. Nonregulated Net Income|10|7990.3|NONREG EXP-MAINTENANCE-KEY SYSTE|-6347.14|-4991.44|-3500|-2038.88|-1400
30|30. Nonregulated Net Income|10|7990.31|NONREG EXP-MAINTENANCE-PBX|-4148.04|-6620.74|-6700|-2283.64|-400
30|30. Nonregulated Net Income|10|7990.4|NONREG EXP-DEPR-R1/B1 TELEPHONE|-24865.72|-14702.33|-29400|0|-9800
30|30. Nonregulated Net Income|10|7990.41|NONREG EXP-DEPR-DATA EQUIP|-1813.14|-1273.52|-1800|0|-600
30|30. Nonregulated Net Income|10|7990.42|NONREG EXP-DEPR-KEY SYST|-18543.68|-7062.12|-15900|0|-5300
30|30. Nonregulated Net Income|10|7990.43|NONREG EXP-DEPR-PBX|-4610.38|-3259.46|-7800|0|-2600
30|30. Nonregulated Net Income|10|7990.54|NONREG EXP-WIRING MAINT-R1-B1 PHO|-52422.13|-40448.1|-54000|-11587.55|-18200
30|30. Nonregulated Net Income|10|7990.55|NONREG EXP-WIRING MAINT-DATA EQUI|-1733.49|-223.17|-400|0|-100
30|30. Nonregulated Net Income|10|7990.56|NONREG EXP-WIRING MAINT-KEY SYSTE|-577.76|-259.2|-300|0|-100
30|30. Nonregulated Net Income|10|7990.57|NONREG EXP-WIRING MAINT-PBX|0|0|0|0|0
30|30. Nonregulated Net Income|10|7990.61|NONREG-COST GOODS SOLD-KEY SYSTE|-16694.92|-8727.64|-9000|-6622.55|-3000
30|30. Nonregulated Net Income|10|7990.62|NONREG-COST GOODS SOLD-PBX|0|0|-900|0|-300
30|30. Nonregulated Net Income|10|7990.63|NONREG-COST GOODS SOLD -DATA EQUI|0|0|0|0|0
30|30. Nonregulated Net Income|10|7990.64|NONREG-COST GOODS SOLD-OTHER|0|-185.94|0|-185.94|0
30|30. Nonregulated Net Income|10|7990.66|NONREG-SELLING/ADMIN EXP-KEY SYST|0|0|0|0|0
30|30. Nonregulated Net Income|10|7990.67|NONREG-SELLING/ADMIN EXP-PBX|-5189.2|-7113.19|-5500|-2434.1|-1900
30|30. Nonregulated Net Income|10|7990.7|NONREG-CUSTOMER SERV EXP-COMMER|-6280.6|-7134.86|-5900|-2561.97|-1900
30|30. Nonregulated Net Income|10|7990.95|OTHER NONREGULATED EXPENSES|0|0|0|0|0
30|30. Nonregulated Net Income|10|7990.99|NONREG-TLC, INC. EXPENSES|-27174.51|-15177.64|-36000|-1912.7|-12000
30|30. Nonregulated Net Income|20|7990.06|MARKETING SERVICES REVENUE|0|0|0|0|0
30|30. Nonregulated Net Income|20|7990.45|TECH SUPPORT REVENUE|29762.48|139004.82|174900|46957.05|59900
30|30. Nonregulated Net Income|21|7990.5|NONREG EXP-WIRING MAINT|0|0|0|0|0
30|30. Nonregulated Net Income|22|7990.02|NONREG SALES-PAGING/INTERCOM SYST|0|0|0|0|0
30|30. Nonregulated Net Income|22|7990.03|NONREG SALES-KEY SYSTEMS|24451.41|0|0|0|0
30|30. Nonregulated Net Income|22|7990.04|NONREG SALES-PBX|293.46|18285.8|21300|7779.48|7100
30|30. Nonregulated Net Income|22|7990.05|NONREG SALES-OTHER|0|0|0|0|0
30|30. Nonregulated Net Income|22|7990.07|NONREG SALES-HOSTED PBX|0|0|0|0|0
30|30. Nonregulated Net Income|22|7990.1|NONREG SALES-RES EQUIP|13.5|9|0|3|0
30|30. Nonregulated Net Income|22|7990.11|NONREG SALES-BUS EQUIP|153.38|46.5|3000|15.5|1000
30|30. Nonregulated Net Income|22|7990.13|WIRE MAINTENANCE- DATA SYSTEM|0|498.26|0|172.31|0
30|30. Nonregulated Net Income|22|7990.21|NONREG INC-LEASED CONN CHARGES|0|0|0|0|0
30|30. Nonregulated Net Income|22|7990.22|NONREG INC-INSIDE WIRING R1-B1 CHGS|172.97|5047.74|3000|1719.81|1000
30|30. Nonregulated Net Income|22|7990.26|NONREG EXP-INSTALL KEY SYSTEMS|-1665|-303.31|0|-303.31|0
30|30. Nonregulated Net Income|22|7990.28|R1 B1 PHONE MAINTENANCE EXP|0|0|0|0|0
30|30. Nonregulated Net Income|22|7990.3|NONREG EXP-MAINTENANCE-KEY SYSTE|0|-191.24|0|-14.98|0
30|30. Nonregulated Net Income|22|7990.5|NONREG EXP-WIRING MAINT|0|0|0|0|0
30|30. Nonregulated Net Income|22|7990.61|COST OF GOODS SOLD-KEY SYSTEMS|-4623.36|-7143.39|-2100|-4649.41|-700
30|30. Nonregulated Net Income|25|7990.5|NONREG EXP-WIRING MAINT|-6240|0|-2400|0|-800
30|30. Nonregulated Net Income|26|7990.02|BEC-PHONE EQUIPMENT SALES|0|0|0|0|0
30|30. Nonregulated Net Income|26|7990.05|NONREG SALES-OTHER|0|0|0|0|0
30|30. Nonregulated Net Income|26|7990.07|NONREG SALES-HOSTED PBX|0|0|0|0|0
30|30. Nonregulated Net Income|26|7990.1|NONREG SALES-RES EQUIP|0|0|0|0|0
30|30. Nonregulated Net Income|26|7990.11|NONREG SALES-BUS EQUIP|0|0|0|0|0
30|30. Nonregulated Net Income|26|7990.13|WIRE MAINTENANCE- DATA SYSTEM|0|499.99|0|199.27|0
30|30. Nonregulated Net Income|26|7990.22|NONREG INC-INSIDE WIRING R1-B1 CHGS|0|0|0|0|0
30|30. Nonregulated Net Income|26|7990.5|NONREG EXP-WIRING MAINT|0|0|0|0|0
32|32. Total Taxes Based on Income|10|7400|FEDERAL TAX EXPENSE|0|0|0|0|0
32|32. Total Taxes Based on Income|20|7400|FEDERAL TAX EXPENSE|-146000|-84000|-27300|0|-9100
32|32. Total Taxes Based on Income|20|7440|TN EXCISE TAX EXP|-44000|-4600|-18900|0|-6300
4|4. Carrier Billing and Collection Revenues|10|5270.2|BILLING & COLLECTION REV|38108.8|32133.2|33400|10692.4|11200
4|4. Carrier Billing and Collection Revenues|20|5270.9|SALES INCOME EQUIPMENT/INVENTORY|0|0|0|0|0
4|4. Carrier Billing and Collection Revenues|22|5270.9|SALES INCOME EQUIPMENT/INVENTORY|0|0|0|0|0
5|5. Miscellaneous Revenues|10|5240|MISC REV-RENT REVENUE|5355.45|5443.23|5400|1814.41|1800
5|5. Miscellaneous Revenues|10|5240.1|MISC REV-RENT-POLE ATTACHMENT|184.5|11204.69|0|187.5|0
5|5. Miscellaneous Revenues|10|5240.2|MISC REV-FIBER LEASE REVENUE|68254.59|68254.59|68100|22751.53|22700
5|5. Miscellaneous Revenues|10|5240.21|MISC REV-FIBER LEASE REV-FIBER IRU|29794.47|29794.47|29700|9931.49|9900
5|5. Miscellaneous Revenues|10|5240.3|MISC REV-CO/RACK SPACE LEASE REVEN|0|0|0|0|0
5|5. Miscellaneous Revenues|10|5240.4|MISC REV-SWITCHING REVENUE|0|0|0|0|0
5|5. Miscellaneous Revenues|10|5260|GIFT CARD SALES|0|0|0|0|0
5|5. Miscellaneous Revenues|10|5264.2|MISC REV-CHECK RETURN CHARGES|2260|2000|2100|340|700
5|5. Miscellaneous Revenues|10|5264.4|MISC REV-LT CHGS-CUSTOMER BLLNG|124063.7|40106.21|114000|0|38000
5|5. Miscellaneous Revenues|10|5264.5|MISC REV-OTH(reconn,numb chng)etc|18360|13235|22500|90|7500
5|5. Miscellaneous Revenues|10|5264.7|MISC REV-E911 BILL-COLLECT|174|174|0|58|0
5|5. Miscellaneous Revenues|10|5264.71|MISC REV-E911 B&C|3183.2|1514.64|1800|563.64|600
5|5. Miscellaneous Revenues|10|5270.9|SALES INCOME EQUIPMENT/INVENTORY|0|0|0|0|0
5|5. Miscellaneous Revenues|20|5200|MISCELLANEOUS REVENUE|0|0|0|0|0
5|5. Miscellaneous Revenues|20|5240|MISC REV-RENT REVENUE|9600|9600|9600|3200|3200
5|5. Miscellaneous Revenues|20|5264.4|MISC REV-LT CHGS-CUSTOMER BLLNG|0|0|0|0|0
5|5. Miscellaneous Revenues|22|5200|MISCELLANEOUS REVENUE|0|0|0|0|0
5|5. Miscellaneous Revenues|22|5264.7|MISC REV-E911 BILL-COLLECT|0|0|0|0|0
5|5. Miscellaneous Revenues|22|5264.73|MISC REV-E911 B&C|0|0|0|0|0
5|5. Miscellaneous Revenues|25|5264.3|MISC REV-CK RET-UNCLAIMED REFND|111|0|0|0|0
5|5. Miscellaneous Revenues|26|5270.9|SALES INCOME EQUIPMENT/INVENTORY|0|0|0|0|0
5|5. Miscellaneous Revenues|28|8500.1|CONSULTING SERVICES REVENUE|0|122251.55|0|122251.55|0
5|5. Miscellaneous Revenues|30|5230.1|MISC REV-DIRECTORY ADVERTISING|78906.76|60237.09|60000|20046.75|20000
5|5. Miscellaneous Revenues|30|5231.1|MISC REV-TV AD INSERTION|3265.08|1911.92|1800|558|600
5|5. Miscellaneous Revenues|30|5231.11|MISC REV-COMMISSIONS|0|366.93|0|122.31|0
5|5. Miscellaneous Revenues|30|5240|MISC REV-RENT REVENUE|0|6000|6000|2000|2000
5|5. Miscellaneous Revenues|30|5800.1|IP VOIP TRUNKS REV|105|105|150|35|50
5|5. Miscellaneous Revenues|30|5800.3|ADVANCED|149.97|149.97|150|49.99|50
6|6. Uncollectible Revenues|10|5301|UNCOLLECTIBLE REV-CUST BILLING|-28100|-45100|-30000|-35600|-10000
6|6. Uncollectible Revenues|30|5301|UNCOLLECTIBLE REV-CUST BILLING|0|0|0|0|0
6|6. Uncollectible Revenues|31|5400|GRANT REVENUES|66970.01|44646.67|66900|0|22300
8|8. Plant Specific Operations Expense|10|6112.1|MOTOR VEHICLE EXP-MAINT & REPAIRS|35496.45|29771.51|34800|9583.48|10900
8|8. Plant Specific Operations Expense|10|6116.1|OTHER WORK EQUIP EXP-TOOLS|15631.97|7892.98|13500|3198.52|4500
8|8. Plant Specific Operations Expense|10|6121|LAND AND BUILDING EXPENSE|143327.85|257650.94|188575|103476.3|58525
8|8. Plant Specific Operations Expense|10|6121.1|LAND & BUILDING EXP-RENT|12600|12600|12600|4200|4200
8|8. Plant Specific Operations Expense|10|6123.1|OFFICE SUPPORT EQUIP EXP|1605.04|1600.65|1650|533.55|550
8|8. Plant Specific Operations Expense|10|6123.2|OFFICE EQUIP EXP-COMPANY COMM EQ|1771.32|2870.8|2175|873.8|725
8|8. Plant Specific Operations Expense|10|6124.1|GEN PURPOSE COMP EXP-DATA PROC|121239.75|123801.01|168300|49125.19|52500
8|8. Plant Specific Operations Expense|10|6124.11|GEN PURPOSE COMP EXP-DP-CONTRACT|99340.62|93413.86|99000|31833.98|33000
8|8. Plant Specific Operations Expense|10|6124.2|GEN PURPOSE COMP EXP-PLANT ENGR|12280.77|8597.64|11350|2531.51|3550
8|8. Plant Specific Operations Expense|10|6212.12|C O SWITCH EXP- SOFTSWITCH|0|0|0|0|0
8|8. Plant Specific Operations Expense|10|6212.15|C O SWITCH EXP-METASWITCH|66656.48|76353.26|82900|27403.85|25300
8|8. Plant Specific Operations Expense|10|6212.3|ALINK/800 DBASE QRY CHGS-INTER|13012.77|11751.44|13500|3882.26|4500
8|8. Plant Specific Operations Expense|10|6212.31|ALINK/800 DBASE QRY CHGS-INTRA|13923.57|12662.24|14700|4185.86|4900
8|8. Plant Specific Operations Expense|10|6232|C O CIRCUIT EQUIP EXPENSE|22782.39|26076.83|88350|7547.54|19450
8|8. Plant Specific Operations Expense|10|6232.1|CIRCUIT EQUIP EXP-TOLL TRUNK CARRI|0|0|0|0|0
8|8. Plant Specific Operations Expense|10|6232.14|CIRCUIT EQUIP EXP-BROADBAND MAINT|125478.8|137697.76|127750|50124.68|43150
8|8. Plant Specific Operations Expense|10|6232.15|CIRCUIT EQUIP EXP-FTTH-FIBER TO THE|288941.72|344680.87|385750|132180.78|122550
8|8. Plant Specific Operations Expense|10|6232.25|CIRCUIT EQUIP EXP-DIGITAL CXR|16969.29|3099.18|11900|1060.51|2800
8|8. Plant Specific Operations Expense|10|6232.31|CIRCUIT EQUIP EXP-EAS CCT RENTAL AG|277.92|277.92|0|92.64|0
8|8. Plant Specific Operations Expense|10|6232.41|CIRCUIT EQUIP EXP-EMG GENERATOR|194.56|3149.5|3800|1444.65|1400
8|8. Plant Specific Operations Expense|10|6232.5|CIRCUIT EQUIP EXP-LGTWV MLPLX|14066.93|11257.11|8250|5565.66|2800
8|8. Plant Specific Operations Expense|10|6232.7|CIRCUIT EQUIP EXP-SPECIAL CIRCUITS|564.39|0|900|0|400
8|8. Plant Specific Operations Expense|10|6232.75|CIRCUIT EQUIP EXP-HI CAP|143.93|0|200|0|0
8|8. Plant Specific Operations Expense|10|6232.8|CIRCUIT EQUIP EXP-TEST EQUIPMENT|268.36|0|1050|0|350
8|8. Plant Specific Operations Expense|10|6411.1|POLES EXPENSE-MAINTENANCE|333035.95|446892.03|419300|136404.34|146800
8|8. Plant Specific Operations Expense|10|6411.2|POLES EXPENSE-POLE RENT|371181.08|362958.82|359550|121080.39|119850
8|8. Plant Specific Operations Expense|10|6421.1|AERIAL CABLE EXPENSE -METALLIC NO|110931.55|67500.38|97300|24584.32|29700
8|8. Plant Specific Operations Expense|10|6421.21|FIBER TERMINATION MAINT EXP|8249.82|11591.08|3600|4374.21|500
8|8. Plant Specific Operations Expense|10|6421.3|AERIAL CABLE EXP-NON METALLIC-TNK|238772.27|202184.17|177000|79333.38|54000
8|8. Plant Specific Operations Expense|10|6422.1|UNDERGND CABLE EXP-METALLIC-NON-|741.76|78.94|450|20.47|150
8|8. Plant Specific Operations Expense|10|6422.3|UNDERGND CABLE EXP-NON METALLIC-|808.98|473.44|900|473.44|300
8|8. Plant Specific Operations Expense|10|6423.1|BURIED CABLE EXP-METALLIC-NON-TRU|70998.21|43086.8|64200|19044.69|21300
8|8. Plant Specific Operations Expense|10|6423.3|BURIED CABLE EXP-NON METLC-FIBER-T|125445.16|161479.76|124950|67631.19|35500
8|8. Plant Specific Operations Expense|10|6441.1|CONDUIT SYSTEMS EXPENSE|0|0|0|0|0
8|8. Plant Specific Operations Expense|20|6112.1|MOTOR VEHICLE EXP-MAINT & REPAIRS|2839.76|2410.24|2350|1228.32|950
8|8. Plant Specific Operations Expense|20|6116.1|OTHER WORK EQUIP EXP-TOOLS|100.13|0|0|0|0
8|8. Plant Specific Operations Expense|20|6121|LAND AND BUILDING EXPENSE|16048.01|26232.15|35055|9200.34|5985
8|8. Plant Specific Operations Expense|20|6121.1|LAND & BUILDING EXP-RENT|0|0|0|0|0
8|8. Plant Specific Operations Expense|20|6123.1|OFFICE SUPPORT EQUIP EXP|0|0|0|0|0
8|8. Plant Specific Operations Expense|20|6123.2|OFFICE SUPPORT EQUIP-COMMUNICATIO|0|0|0|0|0
8|8. Plant Specific Operations Expense|20|6124.1|GEN PURPOSE COMP EXP-DATA PROC|989.71|0|0|0|0
8|8. Plant Specific Operations Expense|20|6232.5|CIRCUIT EQUIP EXP-LGTWV MLPLX|0|216.79|0|0|0
8|8. Plant Specific Operations Expense|20|6232.8|CIRCUIT EQUIP EXP-TEST EQUIPMENT|-64.75|0|0|0|0
8|8. Plant Specific Operations Expense|20|6411.1|POLES EXPENSE-MAINTENANCE|0|0|0|0|0
8|8. Plant Specific Operations Expense|20|6411.21|TOWER RENT EXP|-3947.8|0|0|0|0
8|8. Plant Specific Operations Expense|21|6124.1|GEN PURPOSE COMP EXP-DATA PROC|2550|2685|2550|895|850
8|8. Plant Specific Operations Expense|21|6232.8|CIRCUIT EQUIP EXP-TEST EQUIPMENT|585.07|1413.65|0|975.54|0
8|8. Plant Specific Operations Expense|22|6116.1|OTHER WORK EQUIP EXP-TOOLS|-38.85|8.77|400|0|100
8|8. Plant Specific Operations Expense|22|6121|LAND AND BUILDING EXPENSE|0|795.66|1095|268.26|365
8|8. Plant Specific Operations Expense|22|6123.2|OFFICE SUPPORT EQUIP-COMMUNICATIO|0|0|150|0|50
8|8. Plant Specific Operations Expense|22|6124.1|GEN PURPOSE COMP EXP-DATA PROC|0|885.01|1500|348.4|500
8|8. Plant Specific Operations Expense|22|6232|C O CIRCUIT EQUIP EXPENSE|0|0|0|0|0
8|8. Plant Specific Operations Expense|22|6232.15|CIRCUIT EQUIP EXP-FTTH-FIBER TO THE|1855.95|571.67|2200|0|1300
8|8. Plant Specific Operations Expense|22|6232.41|CIRCUIT EQUIP EXP-EMG GENERATOR|395|0|500|0|0
8|8. Plant Specific Operations Expense|22|6232.5|CIRCUIT EQUIP EXP-LGTWV MLPLX|0|1823.99|0|1823.99|0
8|8. Plant Specific Operations Expense|22|6232.8|CIRCUIT EQUIP EXP-TEST EQUIPMENT|0|0|0|0|0
8|8. Plant Specific Operations Expense|22|6411.1|POLES EXPENSE-MAINTENANCE|1130|238.16|300|238.16|100
8|8. Plant Specific Operations Expense|22|6411.2|POLES EXPENSE-POLE RENT|14895.66|14948.64|13800|4892.66|4600
8|8. Plant Specific Operations Expense|22|6411.21|TOWER RENT EXP|9869.5|6158.58|6300|2052.86|2100
8|8. Plant Specific Operations Expense|22|6421.1|AERIAL CABLE EXPENSE -METALLIC NO|0|0|0|0|0
8|8. Plant Specific Operations Expense|22|6421.21|FIBER TERMINATION MAINT EXP|305.67|0|0|0|0
8|8. Plant Specific Operations Expense|22|6421.3|AERIAL CABLE EXP-NON METALLIC-TNK|5841.55|3635.09|2600|1141.79|400
8|8. Plant Specific Operations Expense|22|6422.3|UNDERGND CABLE EXP-NON METALLIC-|0|0|0|0|0
8|8. Plant Specific Operations Expense|22|6423.1|BURIED CABLE EXP-METALLIC-NON-TRU|0|0|0|0|0
8|8. Plant Specific Operations Expense|22|6423.3|BURIED CABLE EXP-NON METLC-FIBER-T|4339.16|0|1800|0|300
8|8. Plant Specific Operations Expense|22|6441.1|CONDUIT SYSTEMS EXPENSE|0|0|0|0|0
8|8. Plant Specific Operations Expense|25|6116.1|OTHER WORK EQUIP EXP-TOOLS|791.94|0|0|0|0
8|8. Plant Specific Operations Expense|25|6123.2|OFFICE SUPPORT EQUIP-COMMUNICATIO|0|0|150|0|50
8|8. Plant Specific Operations Expense|25|6124.1|GEN PURPOSE COMP EXP-DATA PROC|0|295.7|300|80|100
8|8. Plant Specific Operations Expense|25|6300|SECURITY MONITORING EXPENSE|35373.01|40967.95|37150|13650.83|12700
8|8. Plant Specific Operations Expense|25|6310.1|SECURITY INSTALLATION EXP-RESIDENT|14208.55|33948.09|35400|11655.05|10100
8|8. Plant Specific Operations Expense|25|6310.2|SECURITY INSTALLATION EXP-BUSINESS|30155.57|36934.31|66800|14625.54|18200
8|8. Plant Specific Operations Expense|25|6360.1|SECURITY SYST MAINT & REPAIR-RESIDE|6309.06|9970.06|16500|2834.34|5300
8|8. Plant Specific Operations Expense|25|6360.2|SECURITY SYST MAINT & REPAIR-BUSIN|8058.36|12075.74|13700|2800.26|4300
8|8. Plant Specific Operations Expense|26|6112.1|MOTOR VEHICLE EXP-MAINT & REPAIRS|0|0|250|0|250
8|8. Plant Specific Operations Expense|26|6116.1|OTHER WORK EQUIP EXP-TOOLS|0|0|2000|0|2000
8|8. Plant Specific Operations Expense|26|6121|LAND AND BUILDING EXPENSE|0|1283.76|750|427.92|250
8|8. Plant Specific Operations Expense|26|6123.2|OFFICE SUPPORT EQUIP-COMMUNICATIO|0|0|200|0|100
8|8. Plant Specific Operations Expense|26|6124.1|GEN PURPOSE COMP EXP-DATA PROC|0|0|150|0|50
8|8. Plant Specific Operations Expense|26|6124.11|GEN PURPOSE COMP EXP-DP-CONTRACT|0|0|0|0|0
8|8. Plant Specific Operations Expense|26|6232.15|CIRCUIT EQUIP EXP-FTTH-FIBER TO THE|0|0|200|0|0
8|8. Plant Specific Operations Expense|28|6124.1|GEN PURPOSE COMP EXP-DATA PROC|0|0|0|0|0
8|8. Plant Specific Operations Expense|30|6116.1|OTHER WORK EQUIP EXP-TOOLS|0|0|0|0|0
8|8. Plant Specific Operations Expense|30|6121|LAND AND BUILDING EXPENSE|179.58|164.88|900|54.9|300
8|8. Plant Specific Operations Expense|30|6121.1|LAND & BUILDING EXP-RENT|473.97|473.97|600|157.99|200
8|8. Plant Specific Operations Expense|30|6232|C O CIRCUIT EQUIP EXPENSE|0|0|5000|0|2500
8|8. Plant Specific Operations Expense|30|6411.11|TOWER MAINTENANCE EXP|3904.1|0|1700|0|0
8|8. Plant Specific Operations Expense|30|6411.21|TOWER RENT|6694.59|6972|7050|2324|2350
9|9. Plant Nonspecific Operations Expense|10|6232.4|ACCESS EXPENSE-LNP QUERY|6304.02|7846.96|10950|2593.14|3650
9|9. Plant Nonspecific Operations Expense|10|6512.1|PROVISIONING EXP-CONSTRUCTION MAT|60263.9|75014.94|75600|28706.4|27300
9|9. Plant Nonspecific Operations Expense|10|6530|NETWORK OPERATIONS EXPENSE|8210.73|0|0|0|0
9|9. Plant Nonspecific Operations Expense|10|6531|POWER EXPENSE|79612.99|75921.13|86800|24166.84|28000
9|9. Plant Nonspecific Operations Expense|10|6532.1|NETWORK ADMINISTRATION EXPENSE|98678.62|137070.31|142900|55059.34|36100
9|9. Plant Nonspecific Operations Expense|10|6532.2|NETWORK ADMINISTRATION-NANPA|0|0|0|0|0
9|9. Plant Nonspecific Operations Expense|10|6533.01|TESTING EXPENSE-TEST DESK-GENERAL|11346.43|11683.08|14700|3762.79|4700
9|9. Plant Nonspecific Operations Expense|10|6534.1|PLANT OPERATIONS ADMIN EXPENSE-GE|409416.31|308552.18|425100|116988.33|105200
9|9. Plant Nonspecific Operations Expense|10|6535.1|ENGINEERING EXPENSE-GENERAL|205143.37|237263.69|202300|89808.37|64100
9|9. Plant Nonspecific Operations Expense|10|6540|ACCESS EXPENSE|0|0|0|0|0
9|9. Plant Nonspecific Operations Expense|10|6540.2|INTERSTATE ACCESS EXP-USAC|144759.07|114722.16|120000|37148.05|40000
9|9. Plant Nonspecific Operations Expense|20|6531|POWER EXPENSE|2472.33|0|0|0|0
9|9. Plant Nonspecific Operations Expense|20|6532.2|NETWORK ADMINISTRATION-NANPA|0|0|0|0|0
9|9. Plant Nonspecific Operations Expense|20|6540.2|INTERSTATE ACCESS EXP-USAC|-1741.24|0|0|0|0
9|9. Plant Nonspecific Operations Expense|21|6532.1|NETWORK ADMINISTRATION EXPENSE|0|0|0|0|0
9|9. Plant Nonspecific Operations Expense|21|6540|ACCESS EXPENSE|64417.04|107945.88|134000|36083.95|45000
9|9. Plant Nonspecific Operations Expense|22|6232.4|ACCESS EXPENSE-LNP QUERY|736.04|684.4|900|223.96|300
9|9. Plant Nonspecific Operations Expense|22|6531|POWER EXPENSE|0|2087.24|2200|740.56|850
9|9. Plant Nonspecific Operations Expense|22|6532.1|NETWORK ADMINISTRATION EXPENSE|0|1267.62|2100|408.47|700
9|9. Plant Nonspecific Operations Expense|22|6534.1|PLANT OPERATIONS ADMIN EXPENSE-GE|924.07|571.09|2600|214.09|300
9|9. Plant Nonspecific Operations Expense|22|6535.1|ENGINEERING EXPENSE-GENERAL|19660.06|11782.27|6800|3633.48|500
9|9. Plant Nonspecific Operations Expense|22|6540|ACCESS EXPENSE|123662.43|99262.64|95700|31523.44|31900
9|9. Plant Nonspecific Operations Expense|22|6540.2|INTERSTATE ACCESS EXP-USAC|34308.12|20000.71|39000|4183.63|13000
9|9. Plant Nonspecific Operations Expense|23|6540|ACCESS EXPENSE|28244.99|34465.58|35700|16898.63|11900
9|9. Plant Nonspecific Operations Expense|25|6534.1|PLANT OPERATIONS ADMIN EXPENSE-GE|0|0|0|0|0
9|9. Plant Nonspecific Operations Expense|26|6530|NETWORK OPERATIONS EXPENSE|0|0|0|0|0
9|9. Plant Nonspecific Operations Expense|26|6531|POWER EXPENSE|0|0|0|0|0
9|9. Plant Nonspecific Operations Expense|26|6532.1|NETWORK ADMINISTRATION EXPENSE|0|21.68|150|9|50
9|9. Plant Nonspecific Operations Expense|26|6534.1|PLANT OPERATIONS ADMIN EXPENSE-GE|0|0|1950|0|0
9|9. Plant Nonspecific Operations Expense|26|6535.1|ENGINEERING EXPENSE-GENERAL|0|0|0|0|0
9|9. Plant Nonspecific Operations Expense|26|6540|ACCESS EXPENSE|14078.57|24638.04|26000|8529.94|9000
9|9. Plant Nonspecific Operations Expense|26|6540.2|INTERSTATE ACCESS EXP-USAC|469.5|864.65|1800|182.23|600
9|9. Plant Nonspecific Operations Expense|28|6532.1|NETWORK ADMINISTRATION EXPENSE|4107.34|93.78|0|93.78|0
9|9. Plant Nonspecific Operations Expense|28|6534.1|PLANT OPERATIONS ADMIN EXPENSE-GE|4328.82|0|0|0|0
9|9. Plant Nonspecific Operations Expense|28|6535.1|ENGINEERING EXPENSE-GENERAL|2538.78|0|0|0|0
9|9. Plant Nonspecific Operations Expense|30|6531|POWER EXPENSE|1174.24|1205.77|1350|410.77|450
9|9. Plant Nonspecific Operations Expense|30|6532.1|NETWORK ADMINISTRATION EXPENSE|0|0|0|0|0
9|9. Plant Nonspecific Operations Expense|30|6540|ACCESS EXPENSE|3721.35|3732.84|7050|1244.46|2350
9|9. Plant Nonspecific Operations Expense|30|6540.5|ACCESS EXP-SPECTRUM LEASE CHARGE|0|0|0|0|0
9|9. Plant Nonspecific Operations Expense|50|6535.1|ENGINEERING EXPENSE-GENERAL|2002.5|0|0|0|0
9|9. Plant Nonspecific Operations Expense|50|6540|ACCESS EXPENSE|0|0|0|0|0";

            }

            /// <summary>
            /// The parameters are for example only since no database request is included.
            /// </summary>
            /// <param name="tenantCode">For example only since no database request is included.</param>
            /// <param name="YearMonth">For example only since no database request is included.</param>
            /// <returns></returns>
            public static string NISC_PLSummary(string tenantCode, string YearMonth)
            {
                return @"1|1. Local Network Services Revenue|2496431.3|2263967.43|2297800|750801.37|761000
2|2. Network Access Services Revenue|6596260.42|5663184.45|6256700|1869618.17|2092100
3|3. Long Distance Network Services Revenues|256757.05|270320.91|245100|86265.75|81700
4|4. Carrier Billing and Collection Revenues|38108.8|32133.2|33400|10692.4|11200
5|5. Miscellaneous Revenues|343767.72|372349.29|321300|184000.17|107100
6|6. Uncollectible Revenues|38870.01|-453.33|36900|-35600|12300
8|8. Plant Specific Operations Expense|2430582.16|2666560.78|2777650|963467.34|880500
9|9. Plant Nonspecific Operations Expense|1328840.38|1276698.64|1435650|462613.65|425950
10|10. Depreciation Expense|3581265.47|1877946.55|3148400|0|1049450
11|11. Amortization Expense|29480.19|19653.46|29500|0|9825
12|12. Customer Operations Expense|1087276.43|1075974.8|1572900|383989.41|459200
13|13. Corporate Operations Expense|971606.68|1004487.52|1068950|273149.31|287900
16|16. Other Operating Income and Expenses|0|0|0|0|0
17|17. State and Local Taxes|0|0|0|0|0
18|18. Federal Income Taxes|0|0|0|0|0
19|19. Other Taxes|185400|179540|163350|54740|54450
22|22. Interest on Funded Debt|422003.47|392206.74|420900|132384.14|146300
23|23. Interest Expense - Capital Leases|0|0|0|0|0
24|24. Other Interest Expense|247.88|346.79|0|119.85|0
25|25. Allowance For Funds Used During Construction|652.91|0|0|0|0
27|27. Nonoperating Net Income|4174379.56|4477008.04|3024600|2000032|1051775
28|28. Extraordinary Items|0|0|0|0|0
29|29. Jurisdictional Differences|0|0|0|0|0
30|30. Nonregulated Net Income|136524.01|285788.46|258650|110009.1|89000
32|32. Total Taxes Based on Income|-190000|-88600|-46200|0|-15400
33|33. Retained Earnings or Margins Beginning-of-Year|0|0|0|0|0
34|34. Miscellaneous Credits Year-to-Date|0|0|0|0|0
35|35. Dividends Declared (Common)|0|0|0|0|0
36|36. Dividends Declared (Preferred)|0|0|0|0|0
37|37. Other Debits Year-to-Date|0|0|0|0|0
38|38. Transfers to Patronage Capital|0|0|0|0|0
40|40. Patronage Capital Beginning-of-Year|0|0|0|0|0
41|41. Transfers to Patronage Capital|0|0|0|0|0
42|42. Patronage Capital Credits Retired|0|0|0|0|0
44|44. Annual Debt Service Payments|0|0|0|0|0";
            }
        }
    }
}