
using Interject.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Interject.Models
{
    public class InterjectRowDefItem
    {
        public int Row { get; set; } // The row number index of this item from Excel Report
        public int Column { get; set; } // The column number index of this item from Excel Report
        public string RowDefName { get; set; } // The name of this item
        public List<InterjectColKey> ColKeyList { get; set; } // A list of the ColKeys along with their values
        public string ColumnName { get; set; } // The name of the column of this item from Excel Report
        public Dictionary<string, string> Json { get; set; } = new();

        public void AddJsonEntry(string key, string val)
        {
            Json.Add(key, val);
        }

        public string GetValueString(string name)
        {
            foreach (var key in ColKeyList)
            {
                if(key.Name.Equals(name))
                {
                    return key.Value;
                }
            }
            return "";
        }

        public int GetValueInt(string name)
        {
            foreach (var key in ColKeyList)
            {
                if (key.Name.Equals(name))
                {
                    try
                    {
                        int num = int.Parse(key.Value);
                        return num;
                    }
                    catch(FormatException ex)
                    {
                        Console.WriteLine(ex.Message);
                        return 0;
                    }
                }
            }
            return 0;
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

        /// <summary>
        /// Returns the string representation of this item in XML form
        /// </summary>
        /// <returns></returns>
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