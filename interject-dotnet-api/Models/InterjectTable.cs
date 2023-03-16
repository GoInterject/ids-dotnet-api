using System.Collections.Generic;

namespace Interject.Models
{
    public class InterjectTable
    {
        public string TableName { get; set; } = string.Empty;
        public List<InterjectColumn> Columns { get; private set; } = new();
        public List<List<object>> Rows { get; set; } = new();

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
        }
    }
}