#nullable enable

using Interject.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Interject.DataApi
{
    [ApiController]
    [Authorize]
    [Route("api/v1/[controller]")]
    public class SQLController : ControllerBase
    {
        private readonly ApplicationOptions _options;
        private readonly Dictionary<string, string> _connectionStrings = new();

        public SQLController(ApplicationOptions options, Dictionary<string, string> connectionStrings)
        {
            _options = options;
            _connectionStrings = connectionStrings;
        }

        /// <summary>
        /// Assumes all incoming parameters in the <see cref="InterjectRequest.RequestParameterList"/> are
        /// intended to be passed to a stored procedure.
        /// </summary>
        /// <param name="interjectRequest">
        /// The <see cref="InterjectRequest"/> object to process.
        /// </param>
        [HttpPost]
        [ProducesResponseType(typeof(InterjectResponse), 200)]
        public async Task<IActionResult> Post([FromBody] InterjectRequest interjectRequest)
        {
            InterjectResponse response = new();
            try
            {
                string clientId = string.Empty;
                if (_options.UseClientIdAsConnectionName)
                {
                    clientId = GetClientIdClaim();
                    IActionResult? r = EnforceClientIdSecurity(clientId);
                    if (r != null) return r;
                }

                InterjectRequestHandler handler = new(interjectRequest)
                {
                    IParameterConverter = new SQLParameterConverter(),
                    IDataConnectionAsync = new SqlDataConnectionAsync(interjectRequest, _connectionStrings, clientId),
                    IResponseConverter = new SqlResponseConverter()
                };
                response = await handler.ReturnResponseAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.Write(e.StackTrace);
                response.ErrorMessage = e.Message;
            }
            return Ok(response);
        }

        internal class SQLParameterConverter : IParameterConverter
        {
            /// <summary>
            /// Converts a list of parameters (data type and value) to Microsoft.Data.SqlClient types.
            /// </summary>
            /// <param name="inputParameters"></param>
            /// <param name="outputParameters"></param>
            public void Convert(List<RequestParameter> inputParameters, List<object> outputParameters)
            {
                inputParameters.ForEach((reqParam) =>
                {
                    var p = Convert(reqParam);
                    outputParameters.Add(new ParamPair(p, reqParam));
                });
            }

            /// <summary>
            /// Converts the Interject Parameter Data Type to Microsoft.Data.SqlClient Data Type.
            /// Converts the value by parsing if necessary.
            /// </summary>
            /// <param name="param"></param>
            /// <returns></returns>
            private SqlParameter Convert(RequestParameter param)
            {
                Microsoft.Data.SqlClient.SqlParameter result = new();
                result.ParameterName = param.Name;
                result.Direction = param.ExpectsOutput ? ParameterDirection.InputOutput : ParameterDirection.Input;
                switch (param.DataType)
                {
                    case ParameterDataType.@char:
                        result.DbType = DbType.AnsiStringFixedLength;
                        result.Size = 8000;
                        result.Value = param.InputValue;
                        break;
                    case ParameterDataType.@decimal:
                        result.DbType = DbType.Decimal;
                        result.Size = 0;
                        decimal dec;
                        decimal.TryParse(param.InputValue, out dec);
                        result.Value = dec;
                        break;
                    case ParameterDataType.@double:
                        result.DbType = DbType.Double;
                        result.Size = 0;
                        double dbl;
                        double.TryParse(param.InputValue, out dbl);
                        result.Value = dbl;
                        break;
                    case ParameterDataType.@float:
                        result.DbType = DbType.Double;
                        result.Size = 0;
                        float flt;
                        float.TryParse(param.InputValue, out flt);
                        result.Value = flt;
                        break;
                    case ParameterDataType.@int:
                        result.DbType = DbType.Int32;
                        result.Size = 0;
                        Int32 int32;
                        Int32.TryParse(param.InputValue, out int32);
                        result.Value = int32;
                        break;
                    case ParameterDataType.bigint:
                        result.DbType = DbType.Int64;
                        result.Size = 0;
                        Int64 int64;
                        Int64.TryParse(param.InputValue, out int64);
                        result.Value = int64;
                        break;
                    case ParameterDataType.bit:
                    case ParameterDataType.boolean:
                        result.DbType = DbType.Boolean;
                        result.Size = 0;
                        List<string> truthy = new() { "true", "1", "yes", "y", "t" };
                        List<string> falsey = new() { "false", "0", "no", "n", "f" };
                        var lower = param.InputValue.ToLower();
                        if (truthy.Contains(lower)) param.InputValue = "true";
                        else if (falsey.Contains(lower)) param.InputValue = "false";
                        Boolean boolean;
                        System.Boolean.TryParse(param.InputValue, out boolean);
                        result.Value = boolean;
                        break;
                    case ParameterDataType.date:
                        result.DbType = DbType.Date;
                        result.Size = 0;
                        DateTime dateTime;
                        DateTime.TryParse(param.InputValue, out dateTime);
                        result.Value = dateTime;
                        break;
                    case ParameterDataType.nchar:
                        result.DbType = DbType.StringFixedLength;
                        result.Size = 4000;
                        result.Value = param.InputValue;
                        break;
                    case ParameterDataType.ntext:
                    case ParameterDataType.nvarcharmax:
                        result.DbType = DbType.String;
                        result.Size = -1;
                        result.Value = param.InputValue;
                        break;
                    case ParameterDataType.nvarchar:
                        result.DbType = DbType.String;
                        result.Size = 4000;
                        result.Value = param.InputValue;
                        break;
                    case ParameterDataType.none:
                        result.DbType = DbType.AnsiString;
                        result.Size = 0;
                        result.Value = param.InputValue;
                        break;
                    case ParameterDataType.real:
                    case ParameterDataType.single:
                        result.DbType = DbType.Single;
                        result.Size = 0;
                        float sin;
                        float.TryParse(param.InputValue, out sin);
                        result.Value = sin;
                        break;
                    case ParameterDataType.datetime:
                    case ParameterDataType.smalldatetime:
                        result.DbType = DbType.DateTime;
                        result.Size = 0;
                        DateTime dateTime1;
                        DateTime.TryParse(param.InputValue, out dateTime1);
                        DateTime min = DateTime.Parse("1/1/1900");
                        if (dateTime1 < min) dateTime1 = min;
                        result.Value = dateTime1;
                        break;
                    case ParameterDataType.smallint:
                        result.DbType = DbType.Int16;
                        result.Size = 0;
                        Int16 int16;
                        Int16.TryParse(param.InputValue, out int16);
                        result.Value = int16;
                        break;
                    case ParameterDataType.money:
                    case ParameterDataType.smallmoney:
                        result.DbType = DbType.Currency;
                        result.Size = 0;
                        param.InputValue = param.InputValue.Replace("$", "");
                        double dbl1;
                        double.TryParse(param.InputValue, out dbl1);
                        result.Value = dbl1;
                        break;
                    case ParameterDataType.tinyint:
                        result.DbType = DbType.Byte;
                        result.Size = 0;
                        Int16 tinyInt;
                        Int16.TryParse(param.InputValue, out tinyInt);
                        result.Value = tinyInt < 0 || tinyInt > 255 ? 0 : tinyInt;
                        break;
                    case ParameterDataType.text:
                    case ParameterDataType.varcharmax:
                        result.DbType = DbType.AnsiString;
                        result.Size = -1;
                        result.Value = param.InputValue;
                        break;
                    case ParameterDataType.varchar:
                        result.DbType = DbType.AnsiString;
                        result.Size = 8000;
                        result.Value = param.InputValue;
                        break;
                    default:
                        result.DbType = DbType.AnsiString;
                        result.Size = 0;
                        result.Value = param.InputValue;
                        break;
                }
                if (result.Value == null) result.Value = DBNull.Value;
                return result;
            }
        }

        internal class SqlDataConnectionAsync : IDataConnectionAsync
        {
            /// <summary>
            /// A backward compatiblity feature for supporting a passthrough of a connection string
            /// name for lookup in the appsettings.json or the connection string itself.
            /// </summary>
            private string _connectionString { get; set; }

            /// <summary>
            /// Create an instance of <see cref="SqlDataConnectionAsync"/>
            /// </summary>
            /// <param name="request">The <see cref="InterjectRequest"/> from the http request.</param>
            /// <param name="connectionStrings">A dictionary of key value pairs.</param>
            /// <param name="connectionName">The name of the connection string to use.</param>
            public SqlDataConnectionAsync(InterjectRequest request, Dictionary<string, string> connectionStrings, string connectionName = "")
            {
                connectionStrings ??= new();
                ResolveConnectionString(request, connectionStrings, connectionName);
            }

            /// <summary>
            /// Sets the PassThroughCommand and the connection string. Fetches the connection string in configurations matching its name 
            /// from the PassThroughCommand.ConnectionStringName.
            /// </summary>
            private void ResolveConnectionString(InterjectRequest request, Dictionary<string, string> connectionStrings, string connectionName)
            {
                request.PassThroughCommand = request.PassThroughCommand ?? new();
                var connectionString = string.Empty;

                if (!connectionStrings.TryGetValue(connectionName, out connectionString))
                {
                    connectionStrings.TryGetValue(request.PassThroughCommand.ConnectionStringName, out connectionString);
                }
                else if (string.IsNullOrEmpty(connectionString))
                {
                    // IdsRequest.PassThroughCommand.ConnectionStringName may be the connection string itself.
                    this._connectionString = request.PassThroughCommand.ConnectionStringName;
                }
                else
                {
                    this._connectionString = connectionString;
                }
            }

            /// <summary>
            /// Uses the InterjectRequestHandler pipeline to fetch data asynchronously.
            /// </summary>
            public async Task FetchDataAsync(InterjectRequestHandler handler)
            {
                if (string.IsNullOrEmpty(handler.IdsRequest.PassThroughCommand.ConnectionStringName)) throw new Exception("PassThroughCommand.ConnectionStringName is required.");
                this._connection = new Microsoft.Data.SqlClient.SqlConnection(this._connectionString);
                ConfigureCommand(handler);
                AttachParameters(handler.ConvertedParameters);
                handler.ReturnData = await CallStoredProcedure();
                UpdateOutputParameters(handler.ConvertedParameters, handler.IdsResponse.RequestParameterList);
            }

            private Microsoft.Data.SqlClient.SqlCommand _command { get; set; }
            private SqlConnection _connection { get; set; }

            /// <summary>
            /// Sets this handler's command parameters using the Interject Request.
            /// </summary>
            /// <param name="handler"></param>
            private void ConfigureCommand(InterjectRequestHandler handler)
            {
                this._command = this._connection.CreateCommand();
                this._command.CommandText = handler.IdsRequest.PassThroughCommand.CommandText;
                this._command.CommandTimeout = handler.IdsRequest.PassThroughCommand.CommandTimeout;
                this._command.CommandType = handler.IdsRequest.PassThroughCommand.GetCommandType();
            }

            private void AttachParameters(List<object> convertedParameters)
            {
                convertedParameters.ForEach((param) =>
                {
                    var pair = (ParamPair)param;
                    _command.Parameters.Add(pair.SqlParam);
                });
            }

            private async Task<DataSet> CallStoredProcedure()
            {
                DataSet result = new();
                using (_connection)
                {
                    _command.Connection = _connection;
                    await _connection.OpenAsync();
                    var adapter = new Microsoft.Data.SqlClient.SqlDataAdapter(_command);
                    adapter.Fill(result);
                }
                return result;
            }

            private void UpdateOutputParameters(List<object> convertedParameters, List<RequestParameter> returnParameters)
            {
                returnParameters = new();
                convertedParameters.ForEach((obj) =>
                {
                    var pair = (ParamPair)obj;
                    returnParameters.Add(pair.RequestParameter);
                });
            }
        }

        /// <summary>
        /// Class for holding a pair of parameters, one from the Interject request and its converted SQL parameter
        /// </summary>
        internal class ParamPair
        {
            public SqlParameter SqlParam { get; set; }
            private RequestParameter _baseParam { get; set; }
            public RequestParameter RequestParameter
            {
                get
                {
                    if (_baseParam.ExpectsOutput) _baseParam.OutputValue = SqlParam.SqlValue?.ToString();
                    return _baseParam;
                }
                set
                {
                    _baseParam = value;
                }
            }

            public ParamPair(SqlParameter sqlParam, RequestParameter requestParam)
            {
                this.SqlParam = sqlParam;
                this.RequestParameter = requestParam;
            }
        }

        internal class SqlResponseConverter : IResponseConverter
        {
            public void Convert(InterjectRequestHandler handler)
            {
                var ds = handler.ReturnData as DataSet;
                foreach (DataTable dt in ds.Tables)
                {
                    IdsTable it = InterjectTableFromDataTable(dt);
                    ReturnedData rd = new(it);
                    handler.IdsResponse.ReturnedDataList.Add(rd);
                }
            }

            private IdsTable InterjectTableFromDataTable(DataTable table)
            {
                IdsTable result = new(table.TableName);
                foreach (DataColumn dc in table.Columns)
                {
                    IdsColumn col = InterjectColumnFromDataColumn(dc);
                    result.AddColumn(col);
                }
                foreach (DataRow row in table.Rows)
                {
                    if (row.RowState != DataRowState.Deleted && row.RowState != DataRowState.Detached)
                    {
                        List<string> r = new();
                        foreach (object o in row.ItemArray)
                        {
                            r.Add(o.ToString());
                        }
                        result.Rows.Add(r);
                    }
                }
                return result;
            }

            private IdsColumn InterjectColumnFromDataColumn(DataColumn column)
            {
                IdsColumn result = new();
                result.AllowDBNull = column.AllowDBNull;
                result.AutoIncrement = column.AutoIncrement;
                result.AutoIncrementSeed = column.AutoIncrementSeed;
                result.Caption = column.Caption;
                result.ColumnName = column.ColumnName;
                //result.DataType = column.DataType.Name; // Commented out - Interject works with strings
                var i = (int)column.DateTimeMode;
                result.DateTimeMode = i.ToString();
                result.DefaultValue = column.DefaultValue.ToString();
                result.MaxLength = column.MaxLength;
                //result.Ordinal // Commented out - This is set when added to the table
                result.ReadOnly = column.ReadOnly;
                result.Unique = column.Unique;
                return result;
            }
        }

        private string GetClientIdClaim()
        {
            string userIdentity = User.Claims.FirstOrDefault(c => c.Type == "user_identity")?.Value ?? string.Empty;
            if (string.IsNullOrEmpty(userIdentity))
            {
                return User.Claims.FirstOrDefault(c => c.Type == "ids_client_id")?.Value ?? string.Empty;
            }
            else
            {
                UserIdentityClaim claim = System.Text.Json.JsonSerializer.Deserialize<UserIdentityClaim>(userIdentity);
                return claim.ClientIdPublic;
            }
        }

        private IActionResult? EnforceClientIdSecurity(string clientId)
        {
            IActionResult? result = null;
            if (_options.UseClientIdAsConnectionName)
            {
                if (string.IsNullOrEmpty(clientId) || !_connectionStrings.ContainsKey(clientId))
                {
                    result = new UnauthorizedResult();
                }
            }
            return result;
        }
    }
}