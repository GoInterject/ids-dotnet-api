using System.Collections.Generic;
using System.Text.Json;
namespace Interject.Api
{
    /// <summary>
    /// All controllers must return this type as the response back to the Interject Add-in.
    /// </summary>
    public class InterjectResponse
    {
        public string UserMessage { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public List<RequestParameter> RequestParameterList { get; set; } = new();

        public List<ReturnedData> ReturnedDataList { get; set; } = new();

        public Dictionary<string, string> SupplementalData { get; set; } = new();

        public InterjectResponse() { }

        /// <summary>
        /// Create an instance of <see cref="InterjectResponse"/> with the
        /// <see cref="InterjectRequest.RequestParameterList"/> copied into it.
        /// </summary>
        /// <param name="request">The <see cref="InterjectRequest"/> from the Interject Add-in.</param>
        public InterjectResponse(InterjectRequest request)
        {
            this.RequestParameterList = request.RequestParameterList ?? new();
        }

        /// <summary>
        /// Creates an instance of <see cref="ReturnedData"/> using the table passed in.
        /// </summary>
        /// <remarks>
        /// Note that this serializes the table passed in. Interject add-in expectes it to
        /// be prepared this way.
        /// </remarks>
        /// <param name="table">The table to serialize and add to the ReturnedDataList.</param>
        public void AddReturnedData(IdsTable table)
        {
            string json = JsonSerializer.Serialize(table);
            ReturnedData returnData = new ReturnedData(json);
            ReturnedDataList.Add(returnData);
        }
    }
}