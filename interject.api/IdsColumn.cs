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


namespace Interject.Api
{
    /// <summary>
    /// Based on the Microsoft DataColumn class. This is consumed by Interject and used for displaying data.
    /// </summary>
    /// <remarks>
    /// Defaulted members are not typically important in generating the result for Interject to process.
    /// </remarks>
    public class IdsColumn
    {
        /// <summary>
        /// The name of this column
        /// </summary>
        public string ColumnName { get; set; } = string.Empty;

        /// <summary>
        /// The data type of this column
        /// </summary>
        public string DataType { get; set; } = "String";

        /// <summary>
        /// Allows null values for this column
        /// </summary>
        public bool AllowDBNull { get; set; } = true;

        /// <summary>
        /// If this column's value is to be incremented automatically when a new record is created
        /// </summary>
        public bool AutoIncrement { get; set; } = false;

        /// <summary>
        /// The starting point for this column's value if AutoIncrement is true
        /// </summary>
        public long AutoIncrementSeed { get; set; } = 0;

        /// <summary>
        /// If AutoIncrement is true, then this column increments its value by this value
        /// </summary>
        public int AutoIncrementStep { get; set; } = 1;

        /// <summary>
        /// The display name of this column
        /// </summary>
        public string Caption { get; set; } = string.Empty;

        /// <summary>
        /// The DateTime mode of this column: "Local", "Unspecified", "UnspecifiedLocal", or "Utc".
        /// </summary>
        public string DateTimeMode { get; set; } = "UnspecifiedLocal";

        /// <summary>
        /// The default value for this column
        /// </summary>
        public string DefaultValue { get; set; } = null;

        /// <summary>
        /// The maximum length of the value this column can hold
        /// </summary>
        public int MaxLength { get; set; } = -1;

        /// <summary>
        /// This is set when adding it to an <see cref="IdsTable"/> via <see cref="IdsTable.AddColumn()"/>
        /// </summary>
        public int Ordinal { get; set; }

        /// <summary>
        /// Sets this column as read only status
        /// </summary>
        public bool ReadOnly { get; set; } = false;

        /// <summary>
        /// Sets this column as required to be unique
        /// </summary>
        public bool Unique { get; set; } = false;

        public IdsColumn() { }

        public IdsColumn(string columnName, string dataType = "String")
        {
            this.Caption = columnName;
            this.ColumnName = columnName;
            this.DataType = dataType;
        }
    }
}