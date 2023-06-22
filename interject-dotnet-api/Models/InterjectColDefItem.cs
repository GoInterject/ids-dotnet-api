using Interject.Classes;
using System.Collections.Generic;
using System.Text;

namespace Interject.Models
{
    public class InterjectColDefItem
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public string Value { get; set; }
        public bool? RowDef { get; set; }
        public string ColumnName { get; set; }
        public Dictionary<string, string> Json { get; set; } = new();

        public void AddJsonEntry(string key, string val)
        {
            Json.Add(key, val);
        }

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