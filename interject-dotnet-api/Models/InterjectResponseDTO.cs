using System.Collections.Generic;
using Newtonsoft.Json;

namespace Interject.Models
{
    /// <summary>
    /// All controllers must return this type as the response back to the Interject Add-in.
    /// </summary>
    public class InterjectResponseDTO
    {
        public string UserMessage { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public List<RequestParameter> RequestParameterList { get; set; } = new();

        public List<ReturnedData> ReturnedDataList { get; set; } = new();

        public Dictionary<string, string> SupplementalData { get; set; } = new();

        public InterjectResponseDTO() { }

        /// <summary>
        /// Create an instance of <see cref="InterjectResponseDTO"/> with the
        /// <see cref="InterjectRequestDTO.RequestParameterList"/> copied into it.
        /// </summary>
        /// <param name="request">The <see cref="InterjectRequestDTO"/> from the Interject Add-in.</param>
        public InterjectResponseDTO(InterjectRequestDTO request)
        {
            this.RequestParameterList = request.RequestParameterList ?? (new());
        }

        /// <summary>
        /// Creates an instance of <see cref="ReturnedData"/> using the table passed in.
        /// </summary>
        /// <remarks>
        /// Note that this serializes the table passed in. Interject add-in expectes it to
        /// be prepared this way.
        /// </remarks>
        /// <param name="table">The table to serialize and add to the ReturnedDataList.</param>
        public void AddReturnedData(InterjectTable table)
        {
            string json = JsonConvert.SerializeObject(table);
            ReturnedData returnData = new ReturnedData(json);
            ReturnedDataList.Add(returnData);
        }
    }
}
