
using Interject.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Interject.Models
{
    public class InterjectRowDefItem
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public string RowDefName { get; set; }
        public List<InterjectColKey> ColKeyList { get; set; }
        public string ColumnName { get; set; }
        public Dictionary<string, string> Json { get; set; } = new();

        public void AddJsonEntry(string key, string val)
        {
            Json.Add(key, val);
        }

        public object GetJsonValue(string name, string type = "string")
        {
            if (type.Equals("string", StringComparison.OrdinalIgnoreCase))
            {
                return (string)Json.GetValueOrDefault(name);
            }
            else if (type.Equals("int", StringComparison.OrdinalIgnoreCase))
            {
                var val = Json.GetValueOrDefault(name);
                return int.Parse(val);
            }
            else
            {
                return null;
            }
        }

        public string ToXML()
        {
            StringBuilder sb = new();
            sb.Append("<RowColItem");
            sb.Append($" Row:\"{Row}\"");
            sb.Append($" RowDefName:\"{RowDefName}\"");
            sb.Append($" ColName:\"{ColumnName}\"");
            sb.Append($" JSON:\"{Json.ToJson()}\"");
            if (ColKeyList.Count > 0)
            {
                sb.Append(">");
                foreach (var val in ColKeyList)
                {
                    sb.Append($"{val.ToXML()}");
                }
                sb.Append("</RowColItem>");
            }
            else
            {
                sb.Append("/>");
            }
            return sb.ToString();
        }
    }
}