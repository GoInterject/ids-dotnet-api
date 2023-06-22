namespace Interject.Models
{
    /// <summary>
    /// Based on the Microsoft DataColumn class. This is consumed by Interject and used for displaying data.
    /// </summary>
    /// <remarks>
    /// Defaulted members are not typically important in generating the result for Interject to process.
    /// </remarks>
    public class InterjectColumn
    {
        public string ColumnName { get; set; } = string.Empty;
        public string DataType { get; set; } = "String";
        public bool AllowDBNull { get; set; } = true;
        public bool AutoIncrement { get; set; } = false;
        public long AutoIncrementSeed { get; set; } = 0;
        public int AutoIncrementStep { get; set; } = 1;
        public string Caption { get; set; } = string.Empty;
        public string DateTimeMode { get; set; } = "UnspecifiedLocal";
        public string DefaultValue { get; set; } = null;
        public int MaxLength { get; set; } = -1;
        public int Ordinal { get; set; } // This is set when adding it to an InterjectTable via InterjectTable.AddColumn()
        public bool ReadOnly { get; set; } = false;
        public bool Unique { get; set; } = false;

        public InterjectColumn() { }

        public InterjectColumn(string columnName, string dataType = "String")
        {
            this.Caption = columnName;
            this.ColumnName = columnName;
            this.DataType = dataType;
        }
    }
}