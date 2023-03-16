using System.Collections.Generic;

namespace Interject.Models
{
    public class InterjectResponseDTO
    {
        public string UserMessage { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public List<RequestParameter> RequestParameterList { get; set; } = new();

        public List<ReturnedData> ReturnedDataList { get; set; } = new();

        public Dictionary<string, string> SupplementalData { get; set; } = new();

        public InterjectResponseDTO() { }

        public InterjectResponseDTO(InterjectRequestDTO request)
        {
            this.RequestParameterList = request.RequestParameterList != null ? request.RequestParameterList : new();
        }
    }
}