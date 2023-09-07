namespace Interject.Api
{
    public class RequestParameter
    {
        /// <summary>
        /// The name of this parameter
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The data type of this parameter (see <see cref="ParameterDataType"/>)
        /// </summary>
        public ParameterDataType DataType { get; set; } = ParameterDataType.none;

        /// <summary>
        /// If this parameter is an output parameter
        /// </summary>
        public bool ExpectsOutput { get; set; }

        /// <summary>
        /// This property is not currently supported
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

        /// <summary>
        /// The output value of this parameter
        /// </summary>
        public string OutputValue { get; set; }

        /// <summary>
        /// The validation message for this parmeter
        /// </summary>
        public string UserValidationMessage { get; set; }

        /// <summary>
        /// This property is not currently supported
        /// </summary>
        public string DefaultValue { get; set; }

        public RequestParameter() { }
    }
}