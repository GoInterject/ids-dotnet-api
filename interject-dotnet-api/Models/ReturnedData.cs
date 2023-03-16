namespace Interject.Models
{
    public class ReturnedData
    {
        public object Data { get; set; } = new InterjectTable();
        public int DataFormat { get; set; } = 2; // Hard code for reverse compatibility (DataFormat.JsonTableWithSchema)
        public int SchemaFormat { get; set; } = 1; // Hard code for reverse compatibility (SchemaFormat.Interject_Object)
        public object Schema { get; set; } = new();

        public ReturnedData() { }

        public ReturnedData(object table)
        {
            this.Data = table;
        }
    }
}