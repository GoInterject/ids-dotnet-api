namespace Interject.Api
{
    public class ReturnedData
    {
        /// <summary>
        /// Serialized instance of <see cref="IdsTable"/>
        /// </summary>
        public object Data { get; set; }

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

        /// <summary>
        /// The schema design for the returned data
        /// </summary>
        public object Schema { get; set; }

        public ReturnedData() { }

        public ReturnedData(object table)
        {
            this.Data = table;
        }
    }
}