namespace Interject.Api
{
    public class RequestParameter
    {
        public string Name { get; set; }
        public ParameterDataType DataType { get; set; } = ParameterDataType.none;
        public bool ExpectsOutput { get; set; }

        /// <summary>
        /// This property isn't used.
        /// </summary>
        public bool InFormula { get; set; }

        /// <summary>
        /// Options:
        /// <list type="bullet">
        /// <item>Generic [string]</item>
        /// <item>ExcelVersion [string]</item>
        /// <item>ColDefItems [XML+embeded json]</item>
        /// <item>RowDefItems [XML]</item>
        /// <item>RequestContext [XML]</item>
        /// </list>
        /// </summary>
        public string InputValue { get; set; }
        public string OutputValue { get; set; }
        public string UserValidationMessage { get; set; }

        /// <summary>
        /// Not supported currently.
        /// </summary>
        public string DefaultValue { get; set; }

        public RequestParameter() { }
    }
}