using System.Collections.Generic;
using System.Text;

namespace Interject.Api
{
    /// <summary>
    /// This class represents a single column definition item in the Excel report.
    /// A ColDefRange is designated in the Interject formula.
    /// </summary>
    public class InterjectColDefItem
    {
        /// <summary>
        /// The row number index of this item from Excel Report
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// The column number index of this item from Excel Report
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// The value of this cell in the Excel report
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// If this column is a RowDef column (this will be true if Column is the same number as the RowDefItems)
        /// </summary>
        public bool? RowDef { get; set; }

        /// <summary>
        /// The name of the column of this item from Excel Report (Same as Value)
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Json dictionary that holds the jColumnDef values from Excel Report
        /// </summary>
        public Dictionary<string, string> Json { get; set; } = new();

        /// <summary>
        /// Adds a Json entry into this object's Json dictionary
        /// </summary>
        /// <param name="key">The key of this Json pair</param>
        /// <param name="val">The value of this Json pairt</param>
        public void AddJsonEntry(string key, string val)
        {
            Json.Add(key, val);
        }

        /// <summary>
        /// Returns the string representation of this item in XML form
        /// </summary>
        /// <returns>XML formatted string</returns>
        public string ToXML()
        {
            StringBuilder sb = new();
            sb.Append("<ColDefItem");
            sb.Append($" Row:\"{Row}\"");
            sb.Append($" Col:\"{Column}\"");
            sb.Append($" Val:\"{Value}\"");
            sb.Append($" ColName:\"{ColumnName}\"");
            sb.Append($" JSON:\"{Json.ToJson()}\"/>");
            return sb.ToString();
        }
    }
}