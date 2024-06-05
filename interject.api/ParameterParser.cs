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


using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Xml.XPath;

namespace Interject.Api
{
    internal static class ParameterParser
    {
        /// <summary>
        /// Parses the xml formatted string
        /// </summary>
        /// <param name="xml">The xml string of the Interject_RequestContext parmaeter</param>
        /// <returns><see cref="InterjecRequestContext"/> object</returns>
        public static InterjectRequestContext ParseRequestContext(string xml)
        {
            InterjectRequestContext requestContext = new();

            foreach (XPathNavigator element in GetNodeIterator(xml, "RequestContext"))
            {
                switch (element.Name)
                {
                    case "ExcelVersion":
                        requestContext.ExcelVersion = element.Value;
                        break;
                    case "IdsVersion":
                        requestContext.IdsVersion = element.Value;
                        break;
                    case "FileName":
                        requestContext.FileName = element.Value;
                        break;
                    case "FilePath":
                        requestContext.FilePath = element.Value;
                        break;
                    case "TabName":
                        requestContext.TabName = element.Value;
                        break;
                    case "CellRange":
                        requestContext.CellRange = element.Value;
                        break;
                    case "SourceFunction":
                        requestContext.SourceFunction = element.Value;
                        break;
                    case "UtcOffset":
                        requestContext.UtcOffset = element.Value;
                        break;
                    case "ColDefItems":
                        requestContext.ColDefItems = ParseColDefItems(element);
                        break;
                    case "RowDefItems2":
                        requestContext.RowDefItems = ParseRowDefItems(element);
                        break;
                    case "UserContext":
                        requestContext.UserContext = ParseUserContext(element);
                        break;
                    case "UserContextEncrypted":
                        requestContext.UserContextEcrypted = element.Value;
                        break;
                    case "XMLDataToSave":
                        requestContext.XmlDataToSave = ParseXmlDataToSave(element);
                        break;
                    default:
                        break;
                }
            }

            return requestContext;
        }

        /// <summary>
        /// Parses the xml into a List
        /// </summary>
        /// <param name="navigator"></param>
        /// <returns>A List of <see cref="InterjectColDefItem"/></returns>
        private static List<InterjectColDefItem> ParseColDefItems(XPathNavigator navigator)
        {
            List<InterjectColDefItem> result = new();
            XPathNodeIterator iterator = navigator.Select("Value");
            while (iterator.MoveNext())
            {
                XPathNavigator element = iterator.Current;
                InterjectColDefItem colDefItem = new();

                colDefItem.Row = element.GetAttributeInt("Row");
                colDefItem.Column = element.GetAttributeInt("Column");
                colDefItem.Value = element.GetChildValueString("Name");
                colDefItem.RowDef = element.GetAttributeBool("RowDef");
                colDefItem.ColumnName = element.GetChildValueString("Name");
                colDefItem.Json = element.GetChildValueDictionary("Json");

                result.Add(colDefItem);
            }
            return result;
        }

        /// <summary>
        /// Parses the xml into a List
        /// </summary>
        /// <param name="navigator"></param>
        /// <returns>A List of <see cref="InterjectRowDefItem"/></returns>
        private static List<InterjectRowDefItem> ParseRowDefItems(XPathNavigator navigator)
        {
            List<InterjectRowDefItem> result = new();
            XPathNodeIterator iterator = navigator.Select("Value");
            while (iterator.MoveNext())
            {
                XPathNavigator element = iterator.Current;
                XPathNavigator nodeColKey = element.SelectSingleNode("//ColKey");
                InterjectRowDefItem rowDefItem = new();

                rowDefItem.Row = element.GetAttributeInt("Row");
                rowDefItem.Column = nodeColKey.GetAttributeInt("Column");
                rowDefItem.RowDefName = element.GetAttribute("RowDefName", "");
                rowDefItem.ColKeyList = ParseColKeys(element);
                rowDefItem.ColumnName = nodeColKey.GetAttribute("Name", "");
                rowDefItem.Json = element.GetChildValueDictionary("Json");

                result.Add(rowDefItem);
            }
            return result;
        }

        /// <summary>
        /// Parses the xml into a List
        /// </summary>
        /// <param name="navigator"></param>
        /// <returns>A List of <see cref="IdsColKey"/></returns>
        private static List<IdsColKey> ParseColKeys(XPathNavigator navigator)
        {
            List<IdsColKey> result = new();
            XPathNodeIterator iterator = navigator.Select("ColKey");
            while (iterator.MoveNext())
            {
                XPathNavigator element = iterator.Current;
                IdsColKey colKey = new();

                colKey.Order = element.GetAttributeInt("Order");
                colKey.Column = element.GetAttributeInt("Column");
                colKey.Name = element.GetAttribute("Name", "");
                colKey.Value = element.Value;
                colKey.Json = element.GetChildValueDictionary("Json");

                result.Add(colKey);
            }
            return result;
        }

        /// <summary>
        /// Parses the xml into a List
        /// </summary>
        /// <param name="navigator"></param>
        /// <returns>A List of <see cref="IdsUserContext"/></returns>
        private static IdsUserContext ParseUserContext(XPathNavigator navigator)
        {
            IdsUserContext userContext = new();

            foreach (XPathNavigator element in GetNodeIterator(navigator, "UserContext"))
            {
                switch (element.Name)
                {
                    case "MachineLoginName":
                        userContext.MachineLoginName = element.Value;
                        break;
                    case "MachineName":
                        userContext.MachineName = element.Value;
                        break;
                    case "FullName":
                        userContext.FullName = element.Value;
                        break;
                    case "UserId":
                        userContext.UserId = element.Value;
                        break;
                    case "ClientId":
                        userContext.ClientId = element.Value;
                        break;
                    case "LoginName":
                        userContext.LoginName = element.Value;
                        break;
                    case "LoginAuthTypeId":
                        userContext.LoginAuthTypeId = element.Value;
                        break;
                    case "LoginDateUtc":
                        userContext.LoginDateUtc = element.Value;
                        break;
                    case "UserRoles":
                        XPathNodeIterator rolesIterator = navigator.Select("UserRoles");
                        rolesIterator.MoveNext();
                        userContext.UserRoles = rolesIterator.Current.GetChildValueCSV("Role");
                        break;
                    default:
                        break;
                }
            }

            return userContext;
        }

        /// <summary>
        /// Parses the xml into a Table
        /// </summary>
        /// <param name="xml"></param>
        /// <returns>An <see cref="IdsTable"/></returns>
        public static IdsTable ParseXmlDataToSave(string xml)
        {
            XPathNodeIterator iterator = GetNodeIterator(xml, "XmlDataToSave");
            return ParseXmlDataToSave(iterator);
        }

        private static IdsTable ParseXmlDataToSave(XPathNavigator navigator)
        {
            XPathNodeIterator iterator = GetNodeIterator(navigator, "XmlDataToSave");
            return ParseXmlDataToSave(iterator);
        }

        private static IdsTable ParseXmlDataToSave(XPathNodeIterator nodeIterator)
        {
            IdsTable table = new();

            foreach (XPathNavigator element in nodeIterator)
            {
                switch (element.Name)
                {
                    case "c":
                        string col = element.GetAttribute("Column", "");
                        table.AddColumn(new(col));
                        break;
                    case "r":
                        List<string> row = new();
                        XPathNodeIterator iterator = element.SelectDescendants(XPathNodeType.Element, false);
                        foreach (XPathNavigator rowElement in iterator)
                        {
                            row.Add(rowElement.Value);
                        }
                        table.Rows.Add(row);
                        break;
                    default:
                        break;
                }
            }

            return table;
        }

        #region Utility Functions

        private static XPathNodeIterator GetNodeIterator(string xml, string name)
        {
            using TextReader reader = new StringReader(xml);
            XPathDocument xpdoc = new XPathDocument(reader);
            XPathNavigator docNavigator = xpdoc.CreateNavigator();
            return GetNodeIterator(docNavigator, name);
        }

        private static XPathNodeIterator GetNodeIterator(XPathNavigator navigator, string name)
        {
            XPathNodeIterator root = navigator.Select(name);
            root.MoveNext();
            XPathNavigator rootNavigator = root.Current;
            return rootNavigator.SelectChildren(XPathNodeType.Element);
        }

        private static int GetAttributeInt(this XPathNavigator navigator, string name)
        {
            if (int.TryParse(navigator.GetAttribute(name, ""), out int outVal))
            {
                return outVal;
            }
            return 0;
        }

        private static bool? GetAttributeBool(this XPathNavigator navigator, string name)
        {
            string value = navigator.GetAttribute(name, "");
            if (string.IsNullOrEmpty(value)) return null;
            bool.TryParse(value, out bool result);
            return result;
        }

        private static string GetChildValueCSV(this XPathNavigator navigator, string name)
        {
            var result = new StringBuilder();
            XPathNodeIterator iterator = navigator.Select(name);
            while (iterator.MoveNext())
            {
                if (result.Length > 0)
                {
                    result.Append(",");
                }
                result.Append(iterator.Current.Value);
            }
            return result.ToString();
        }

        private static string GetChildValueString(this XPathNavigator navigator, string name)
        {
            var result = string.Empty;
            XPathNodeIterator iterator = navigator.Select(name);
            if (iterator.Count > 0)
            {
                iterator.MoveNext();
                result = iterator.Current.Value;
            }
            return result;
        }

        private static Dictionary<string, string> GetChildValueDictionary(this XPathNavigator navigator, string name)
        {
            Dictionary<string, string> result = new();

            XPathNodeIterator iterator = navigator.Select(name);
            if (iterator.Count < 1) return result;
            iterator.MoveNext();
            var json = iterator.Current.Value;
            result = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            return result;
        }

        #endregion
    }
}
