// Copyright 2024 Interject Data Systems

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.


using System;
using System.Collections.Generic;
using System.Text;

namespace Interject.Api
{
    /// <summary>
    /// This class represents a single row definition item in the Excel report.
    /// A RowDefRange is designated in the Interject formula.
    /// </summary>
    public class InterjectRowDefItem
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
        /// The name of this item
        /// </summary>
        public string RowDefName { get; set; }

        /// <summary>
        /// A list of the <see cref="IdsColKey">IdsColKeys/> along with their values
        /// </summary>
        public List<IdsColKey> ColKeyList { get; set; }

        /// <summary>
        /// The name of the column of this item from Excel Report
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

        /// <param name="name">The name to look up in the list of IdsColKeys</param>
        /// <returns>The value of this IdsColKey</returns>
        public string GetValueString(string name)
        {
            foreach (var key in ColKeyList)
            {
                if (key.Name.Equals(name))
                {
                    return key.Value;
                }
            }
            return "";
        }

        /// <param name="name">The name to look up in the list of IdsColKeys</param>
        /// <returns>The int value of this IdsColKey</returns>
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
                    catch (FormatException ex)
                    {
                        Console.WriteLine(ex.Message);
                        return 0;
                    }
                }
            }
            return 0;
        }


        /// <summary>
        /// Parses a string as a Json and returns the value based on the type
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns>A string or an int</returns>
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