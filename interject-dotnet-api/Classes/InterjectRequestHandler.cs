using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Interject.Classes
{
    /// <summary>
    /// This is a container for the pipeline logic where Interject Requests are processed.<br/>
    /// </summary>
    public class InterjectRequestHandler
    {
        #region Interface members

        /// <summary>
        /// An implementation of the <see cref="IParameterConverter"/> interface.
        /// </summary>
        public IParameterConverter ParameterConverter { get; set; } = new DefaultParameterConverter();

        /// <summary>
        /// An implementation of the <see cref="IDataConnection"/> interface.
        /// </summary>
        public IDataConnection DataConnection { get; set; } = new DefaultDataConnection();

        /// <summary>
        /// An implementation of the <see cref="IDataConnection"/> interface.
        /// </summary>
        public IDataConnectionAsync DataConnectionAsync { get; set; } = new DefaultDataConnectionAsync();

        /// <summary>
        /// An implementation of the <see cref="IResponseConverter"/> interface.
        /// </summary>
        public IResponseConverter ResponseConverter { get; set; } = new DefaultResponseConverter();

        #endregion

        #region Default interface implementations

        private class DefaultParameterConverter : IParameterConverter
        {
            public void Convert(List<RequestParameter> inputParameters, List<object> outputParameters)
            {
                inputParameters.ForEach((param) => { outputParameters.Add(param); });
            }
        }

        private class DefaultDataConnection : IDataConnection
        {
            public void FetchData(InterjectRequestHandler handler) { } // Do nothing
        }

        private class DefaultDataConnectionAsync : IDataConnectionAsync
        {
            public async Task FetchDataAsync(InterjectRequestHandler handler)
            {
                await Task.Run(() => { });
            }
        }

        internal class DefaultResponseConverter : IResponseConverter
        {
            public void Convert(InterjectRequestHandler handler) { } // Do nothing
        }

        #endregion

        #region Data storage members

        /// <summary>
        /// A container for storing request parameter data after processing them in the
        /// <see cref="IParameterConverter"/>. This is useful for tracking the incomming
        /// request throughout the pipeline.
        /// </summary>
        public List<object> ConvertedParameters { get; set; } = new();

        /// <summary>
        /// The request coming from the client call.
        /// </summary>
        public InterjectRequest IdsRequest { get; set; }

        /// <summary>
        /// The response object is configured during the Init method to include the original
        /// request's list of request parameters.
        /// </summary>
        public InterjectResponse IdsResponse { get; set; }

        /// <summary>
        /// A place to store data returned from the fetch phase of the pipeline. This is intended
        /// to be accessed again during the convert phase of the pipeline when the data returned
        /// is transformed into standard<see cref="InterjectTable"/>data within the
        /// <see cref="InterjectResponse.ReturnedDataList"/> collection.
        /// </summary>
        public object ReturnData { get; set; }

        #endregion

        public InterjectRequestHandler(InterjectRequest request)
        {
            this.IdsRequest = request;
            if (request.RequestParameterList == null) request.RequestParameterList = new();
            this.IdsResponse = new(request);
        }

        public InterjectResponse ReturnResponse()
        {
            this.ParameterConverter.Convert(this.IdsRequest.RequestParameterList, this.ConvertedParameters);
            this.DataConnection.FetchData(this);
            this.ResponseConverter.Convert(this);
            return this.PackagedResponse;
        }

        public async Task<InterjectResponse> ReturnResponseAsync()
        {
            this.ParameterConverter.Convert(this.IdsRequest.RequestParameterList, this.ConvertedParameters);
            await this.DataConnectionAsync.FetchDataAsync(this);
            this.ResponseConverter.Convert(this);
            return this.PackagedResponse;
        }

        /// <summary>
        /// Performs final serialization required for the addin to consume the response.
        /// </summary>
        private InterjectResponse PackagedResponse
        {
            get
            {
                this.IdsResponse.ReturnedDataList.ForEach((returnData) =>
                {
                    string json = JsonConvert.SerializeObject(returnData.Data);
                    returnData.Data = json;
                });
                return this.IdsResponse;
            }
        }
    }
}