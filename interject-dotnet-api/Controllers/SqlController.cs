using Interject.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Interject.API
{
    // [Authorize] //security is currently out of scope of the project. This will be added at a later phase prior to production use.
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SQLController : ControllerBase
    {
        private readonly ConnectionStringOptions _connectionStringOptions;
        public SQLController(ConnectionStringOptions options)
        {
            _connectionStringOptions = options;
        }

        /// <summary>
        /// The principal method of processing an action in the Interject Addin. E.G. Pull, Save, Drill
        /// </summary>
        /// <param name="interjectRequest">
        /// The <see cref="InterjectRequest"/> object to process.
        /// </param>
        [HttpPost]
        [ProducesResponseType(typeof(InterjectResponse), 200)]
        public async Task<InterjectResponse> Post([FromBody] InterjectRequest interjectRequest)
        {
            InterjectRequestHandler handler = new(interjectRequest);
            handler.ParameterConverter = new SQLParameterConverter();
            handler.DataConnection = new SqlDataConnection(interjectRequest, _connectionStringOptions);
            handler.ResponseConverter = new SqlResponseConverter();
            return handler.PackagedResponse;

            // handler.ConvertParameters(new SQLParameterConverter());
            // await handler.FetchDataAsync(new SqlDataConnection(interjectRequest, _connectionStringOptions));
            // handler.ConvertResponseData(new SqlResponseConverter());
            // return handler.PackagedResponse;
        }

        public class SQLParameterConverter : IParameterConverter
        {
            public void Convert(InterjectRequestHandler handler)
            {
                handler.IdsRequest.RequestParameterList.ForEach((reqParam) =>
                {
                    var p = Convert(reqParam);
                    handler.ConvertedParameters.Add(new ParamPair(p, reqParam));
                });
            }

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

        public class SqlDataConnection : IDataConnection
        {
            private PassThroughCommand passThroughCommand { get; set; }
            /// <summary>
            /// A backward compatiblity feature for supporting a passthrough of a connection string
            /// name for lookup in the appsettings.json or the connection string itself.
            /// </summary>
            public string ConnectionString { get; set; }

            /// <summary>
            /// Provided by dependancy injection during the application startup. This is coming
            /// from the appSettings.json "Connections" collection property.
            /// </summary>
            private List<ConnectionDescriptor> _connectionStrings;

            /// <summary>
            /// Create an instance of <see cref="InterjectRequestHandler"/>
            /// </summary>
            /// <param name="connectionStringOptions"></param>
            public SqlDataConnection(InterjectRequest request, ConnectionStringOptions connectionStringOptions)
            {
                if (connectionStringOptions == null)
                {
                    _connectionStrings = new();
                }
                else if (connectionStringOptions.ConnectionStrings == null)
                {
                    _connectionStrings = new();
                }
                else
                {
                    _connectionStrings = connectionStringOptions.ConnectionStrings;
                }
                ResolveConnectionString(request);
            }

            private void ResolveConnectionString(InterjectRequest request)
            {
                if (request.PassThroughCommand == null) request.PassThroughCommand = new();
                var conStrDesc = _connectionStrings.FirstOrDefault(cs => cs.Name == request.PassThroughCommand.ConnectionStringName);

                if (conStrDesc == null)
                {
                    // IdsRequest.PassThroughCommand.ConnectionStringName 
                    // may be the connection string itself.
                    this.ConnectionString = request.PassThroughCommand.ConnectionStringName;
                }
                else
                {
                    this.ConnectionString = conStrDesc.ConnectionString;
                }
            }

            public async Task FetchDataAsync(InterjectRequestHandler handler)
            {
                if (string.IsNullOrEmpty(handler.IdsRequest.PassThroughCommand.ConnectionStringName)) throw new UserException("PassThroughCommand.ConnectionStringName is required.");
                this._connection = new Microsoft.Data.SqlClient.SqlConnection(handler.ConnectionString);
                ConfigureCommand(handler.IdsRequest.PassThroughCommand);
                AttachParameters(handler);
                await CallStoredProcedure(handler);
                UpdateOutputParameters(handler);
            }

            private Microsoft.Data.SqlClient.SqlCommand _command { get; set; }
            private SqlConnection _connection { get; set; }

            private void ConfigureCommand(PassThroughCommand passThroughCommand)
            {
                this._command = this._connection.CreateCommand();
                this._command.CommandText = passThroughCommand.CommandText;
                this._command.CommandTimeout = passThroughCommand.CommandTimeout;
                this._command.CommandType = passThroughCommand.GetCommandType();
            }

            private void AttachParameters(InterjectRequestHandler handler)
            {
                handler.ConvertedParameters.ForEach((param) =>
                {
                    var pair = (ParamPair)param;
                    this._command.Parameters.Add(pair.SqlParam);
                });
            }

            private async Task CallStoredProcedure(InterjectRequestHandler handler)
            {
                DataSet ds = new();
                using (this._connection)
                {
                    this._command.Connection = this._connection;
                    await this._connection.OpenAsync();
                    var adapter = new Microsoft.Data.SqlClient.SqlDataAdapter(this._command);
                    adapter.Fill(ds);
                }
                handler.ReturnData = ds;
            }

            private void UpdateOutputParameters(InterjectRequestHandler handler)
            {
                List<RequestParameter> rps = new();
                handler.ConvertedParameters.ForEach((obj) =>
                {
                    var pair = (ParamPair)obj;
                    rps.Add(pair.RequestParameter);
                });
                handler.IdsResponse.RequestParameterList = rps;
            }

            public void FetchData(InterjectRequestHandler handler)
            {
                throw new NotImplementedException();
            }
        }

        public class ParamPair
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

        public class SqlResponseConverter : IResponseConverter
        {
            public void Convert(InterjectRequestHandler handler)
            {
                var ds = handler.ReturnData as DataSet;
                foreach (DataTable dt in ds.Tables)
                {
                    InterjectTable it = InterjectTableFromDataTable(dt);
                    ReturnedData rd = new(it);
                    handler.IdsResponse.ReturnedDataList.Add(rd);
                }
            }

            private InterjectTable InterjectTableFromDataTable(DataTable table)
            {
                InterjectTable result = new(table.TableName);
                foreach (DataColumn dc in table.Columns)
                {
                    InterjectColumn col = InterjectColumnFromDataColumn(dc);
                    result.AddColumn(col);
                }
                foreach (DataRow row in table.Rows)
                {
                    if (row.RowState != DataRowState.Deleted && row.RowState != DataRowState.Detached)
                    {
                        result.Rows.Add(row.ItemArray.ToList());
                    }
                }
                return result;
            }

            private InterjectColumn InterjectColumnFromDataColumn(DataColumn column)
            {
                InterjectColumn result = new();
                result.AllowDBNull = column.AllowDBNull;
                result.AutoIncrement = column.AutoIncrement;
                result.AutoIncrementSeed = column.AutoIncrementSeed;
                result.Caption = column.Caption;
                result.ColumnName = column.ColumnName;
                result.DataType = column.DataType.Name;
                var i = (int)column.DateTimeMode;
                result.DateTimeMode = i.ToString();
                result.DefaultValue = column.DefaultValue.ToString();
                result.MaxLength = column.MaxLength;
                //result.Ordinal ~~ This is set when added to the table
                result.ReadOnly = column.ReadOnly;
                result.Unique = column.Unique;
                return result;
            }
        }
    }
}