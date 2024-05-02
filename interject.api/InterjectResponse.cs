// Copyright 2024 Interject Data Systems

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.


using System.Collections.Generic;
using System.Text.Json;

namespace Interject.Api
{
    /// <summary>
    /// All controllers must return this type as the response back to the Interject Add-in.
    /// </summary>
    public class InterjectResponse
    {
        /// <summary>
        /// A message of the results of the request
        /// </summary>
        public string UserMessage { get; set; } = string.Empty;

        /// <summary>
        /// A message of the error result of the request (if applicable)
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// A list of the original <see cref="RequestParameter">Request Parameters</see>
        /// </summary>
        public List<RequestParameter> RequestParameterList { get; set; } = new();

        /// <summary>
        /// A list of the data returned from the api
        /// </summary>
        public List<ReturnedData> ReturnedDataList { get; set; } = new();

        /// <summary>
        /// A dictionary list of supplemental data returned from the api
        /// </summary>
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
