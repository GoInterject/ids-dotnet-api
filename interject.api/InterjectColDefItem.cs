using System.Collections.Generic;
using System.Text;

namespace Interject.Api
{
    public class InterjectColDefItem
    {
        public int Row { get; set; } // The row number index of this item from Excel Report
        public int Column { get; set; } // The column number index of this item from Excel Report
        public string Value { get; set; } // The name of the column of this item from Excel Report
        public bool? RowDef { get; set; } // If this column is a RowDef column (this will be true if Column is the same number as the RowDefItems)
        public string ColumnName { get; set; } // Same as Value
        public Dictionary<string, string> Json { get; set; } = new(); // Json dictionary that holds the jColumnDef values from Excel Report

        public void AddJsonEntry(string key, string val)
        {
            Json.Add(key, val);
        }

        /// <summary>
        /// Returns the string representation of this item in XML form
        /// </summary>
        /// <returns></returns>
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