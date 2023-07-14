namespace Interject.Models
{
    public class ReturnedData
    {
        /// <summary>
        /// Serialized instance of <see cref="InterjectTable"/>.
        /// </summary>
        public object Data { get; set; } = new InterjectTable();

        /// <summary>
        /// Hard coded for reverse compatibility (DataFormat.JsonTableWithSchema)
        /// </summary>
        /// <value>2</value>
        public int DataFormat { get; set; } = 2;

        /// <summary>
        /// Hard code for reverse compatibility (SchemaFormat.Interject_Object)
        /// </summary>
        /// <value>1</value>
        public int SchemaFormat { get; set; } = 1;

        public object Schema { get; set; }

        public ReturnedData() { }

        public ReturnedData(object table)
        {
            this.Data = table;
        }
    }
}