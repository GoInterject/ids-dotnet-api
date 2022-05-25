using System.Collections.Generic;

namespace Interject.Classes
{
    /// <summary>
    /// All requests to the Data Engine API must use the<see cref="InterjectRequest"/>class as the body of the request.
    /// </summary>
    public class InterjectRequest
    {
        public string DataPortalName { get; set; }
        public List<RequestParameter> RequestParameters { get; set; }
        public PassThroughCommand PassThroughCommand { get; set; }

        public InterjectRequest() { }
    }

    public class RequestParameter
    {
        public string Name { get; set; }
        public ParameterDataType DataType { get; set; }
        public bool ExpectsOutput { get; set; }
        public bool InFormula { get; set; }
        public string InputValue { get; set; }
        public string OutputValue { get; set; }
        public string UserValidationMessage { get; set; }
        public string DefaultValue { get; set; }

        public RequestParameter() { }
    }

    public class PassThroughCommand
    {
        public string ConnectionStringName { get; set; }
        public CommandType CommandType { get; set; }
        public string CommandText { get; set; }
        public int CommandTimeout { get; set; }

        public System.Data.CommandType GetCommandType()
        {
            if (this.CommandType == CommandType.TableDirect)
            {
                return System.Data.CommandType.TableDirect;
            }
            else if (this.CommandType == CommandType.Text)
            {
                return System.Data.CommandType.Text;
            }
            else
            {
                return System.Data.CommandType.StoredProcedure;
            }
        }
    }

    public class InterjectResponse
    {
        public string UserMessage { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public List<RequestParameter> RequestParameterList { get; set; } = new();

        public List<ReturnedData> ReturnedDataList { get; set; } = new();

        public Dictionary<string, string> SupplementalData { get; set; } = new();

        public InterjectResponse() { }

        public InterjectResponse(InterjectRequest request)
        {
            this.RequestParameterList = request.RequestParameters != null ? request.RequestParameters : new();
        }
    }

    public class ReturnedData
    {
        public InterjectTable Data { get; set; } = new();
        public readonly int DataFormat = 2; // Hard code for reverse compatibility (DataFormat.JsonTableWithSchema)
        public readonly int SchemaFormat = 1; // Hard code for reverse compatibility (SchemaFormat.Interject_Object)

        public ReturnedData() { }

        public ReturnedData(InterjectTable table)
        {
            this.Data = table;
        }
    }

    public class InterjectTable
    {
        public string TableName { get; set; }
        public List<InterjectColumn> Columns { get; private set; } = new();
        public List<List<object>> Rows { get; set; } = new();

        public InterjectTable() { }

        public InterjectTable(string tableName)
        {
            this.TableName = tableName;
        }

        /// <summary>
        /// Sets the <see cref="InterjectColumn.Ordinal"/> with respect to the collection it is being added to.
        /// </summary>
        /// <param name="column"></param>
        public void AddColumn(InterjectColumn column)
        {
            column.Ordinal = this.Columns.Count;
            this.Columns.Add(column);
        }
    }

    /// <summary>
    /// Based on the Microsoft DataColumn class. This is consumed by Interject and used for displaying data.
    /// </summary>
    /// <remarks>
    /// Defaulted members are not typically important in generating the result for Interject to process.
    /// </remarks>
    public class InterjectColumn
    {
        public bool AllowDBNull { get; set; } = true;
        public bool AutoIncrement { get; set; } = false;
        public long AutoIncrementSeed { get; set; } = 0;
        public int AutoIncrementStep { get; set; } = 1;
        public string Caption { get; set; }
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public string DateTimeMode { get; set; } = "UnspecifiedLocal";
        public string DefaultValue { get; set; } = null;
        public int MaxLength { get; set; } = -1;
        public int Ordinal { get; set; }
        public bool ReadOnly { get; set; } = false;
        public bool Unique { get; set; } = false;

        public InterjectColumn() { }

        public InterjectColumn(string columnName)
        {
            this.ColumnName = columnName;
        }
    }
}