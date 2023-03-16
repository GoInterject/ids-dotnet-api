using Interject.Classes;

namespace Interject.Models
{
    public class RequestParameter
    {
        public string Name { get; set; }
        public ParameterDataType DataType { get; set; }
        public bool ExpectsOutput { get; set; }
        public bool InFormula { get; set; }
        public string InputValue { get; set; }
        public string OutputValue { get; set; }
        public string UserValidationMessage { get; set; }
        public string DefaultValue { get; set; }

        public RequestParameter() { }
    }
}