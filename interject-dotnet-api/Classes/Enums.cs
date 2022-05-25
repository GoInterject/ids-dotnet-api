namespace Interject.Classes
{
    public enum CommandType
    {
        None = 0,
        StoredProcedure = 1,
        TableDirect = 2,
        Text = 3
    }

    // public enum DataFormat
    // {
    //     None = 0,
    //     XmlString = 1,
    //     JsonTableWithSchema = 2,
    //     JsonTable = 3,
    //     DataSet = 4
    // }

    public enum ParameterDataType
    {
        none = 0,
        varchar = 1,
        varcharmax = 2,
        nvarchar = 3,
        nvarcharmax = 4,
        text = 5,
        ntext = 6,
        @char = 7,
        nchar = 8,
        bigint = 9,
        @int = 10,
        smallint = 11,
        tinyint = 12,
        boolean = 13,
        bit = 14,
        money = 15,
        smallmoney = 16,
        @float = 17,
        real = 18,
        single = 19,
        @double = 20,
        @decimal = 21,
        datetime = 22,
        smalldatetime = 23,
        date = 24
    }
}