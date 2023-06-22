using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interject.Models
{
    public class InterjectTable
    {
        public string TableName { get; set; } = string.Empty;
        public List<InterjectColumn> Columns { get; private set; } = new();
        public List<List<string>> Rows { get; set; } = new();

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
            this.Rows.ForEach(row =>
            {
                row.Add(string.Empty);
            });
        }

        /// <summary>
        /// Add a column to the table. This also adds a null value
        /// to each existing row.
        /// </summary>
        /// <param name="row"></param>
        public void AddRow(List<string> row)
        {
            RightSizeNewRow(row);
            this.Rows.Add(row);
        }

        /// <summary>
        /// Add a collection of data at the last position of the table data.
        /// </summary>
        /// <param name="row"></param>
        public void AddRowAtEnd(List<string> row)
        {
            RightSizeNewRow(row);
            this.Rows.Insert(this.Rows.Count, row);
        }

        private void RightSizeNewRow(List<string> row)
        {
            while (row.Count < this.Columns.Count)
            {
                row.Add(string.Empty);
            }
            while (row.Count > this.Columns.Count)
            {
                row.RemoveAt(row.Count - 1);
            }
        }

        /// <summary>
        /// Replace the value of the column matching the column argument at the
        /// index specified by the row argument with the value argument.
        /// </summary>
        /// <param name="column">The column to match.</param>
        /// <param name="row">The row index.</param>
        /// <param name="value">The new value.</param>
        public void Update(string column, int row, string value)
        {
            int colIndex = GetColumnIndex(column);
            if (colIndex > -1 && row <= this.Rows.Count)
            {
                this.Rows[row][colIndex] = value;
            }
        }

        /// <summary>
        /// Searches the ColumnName value of each column.
        /// </summary>
        /// <param name="name">The ColumnName to search for.</param>
        /// <param name="caseSensitive">Option for case sensitivity of the search.</param>
        /// <returns>
        /// True if a column matching the name parameter is found.
        /// False if not.
        /// </returns>
        public bool HasColumn(string name, bool caseSensitive = true)
        {
            if (caseSensitive)
            {
                return this.Columns.Any(c => c.ColumnName == name);
            }
            else
            {
                return this.Columns.Any(c => c.ColumnName.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// Remove a column and all the row data for that column.
        /// </summary>
        /// <param name="name">The name of the column to remove.</param>
        public void DeleteColumn(string name)
        {
            int colIndex = GetColumnIndex(name);
            if (colIndex > -1)
            {
                this.Columns.RemoveAt(colIndex);
                this.Rows.ForEach(row =>
                {
                    row.RemoveAt(colIndex);
                });
            }
        }

        /// <param name="name">The name of the column to search for.</param>
        /// <returns>The index of the column if found, -1 if no match is found.</returns>
        public int GetColumnIndex(string name)
        {
            int result = -1;
            for (int i = 0; i < this.Columns.Count; i++)
            {
                if (this.Columns[i].ColumnName.Equals(name, System.StringComparison.OrdinalIgnoreCase)) result = i;
            }
            return result;
        }

        /// <returns>The collection of data in the last position of the table.</returns>
        public List<string> GetLastRow()
        {
            if (this.Rows.Count > 0)
            {
                return this.Rows[this.Rows.Count - 1];
            }
            else
            {
                return null;
            }
        }

        public override string ToString()
        {
            StringBuilder tableSb = new();
            tableSb.AppendLine($"TableName: {this.TableName}");

            StringBuilder colSb = new();
            this.Columns.ForEach((col) =>
            {
                colSb.Append($"{col.ColumnName}, ");
            });
            tableSb.AppendLine(colSb.ToString());

            this.Rows.ForEach((row) =>
            {
                StringBuilder rowSb = new();
                row.ForEach((cell) =>
                {
                    rowSb.Append($"{(string)cell} ,");
                });
                tableSb.AppendLine(rowSb.ToString());
            });
            return tableSb.ToString();
        }
    }
}