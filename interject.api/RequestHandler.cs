using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

namespace Interject.Api
{
    #region Interfaces

    public interface IParameterConverter
    {
        /// <summary>
        /// Process the incomming <see cref="RequestParameter"/> collection in the <see cref="InterjectRequest"/>.
        /// </summary>
        /// <param name="inputParameters">The list of parameters from the original request.</param>
        /// <param name="outputParameters">Output parameters that can be used by the <see cref="IDataConnection"/> implementation.</param>
        public void Convert(List<RequestParameter> inputParameters, List<object> outputParameters);
    }

    public interface IDataConnection
    {
        public void FetchData(InterjectRequestHandler handler);
    }

    public interface IDataConnectionAsync
    {
        public Task FetchDataAsync(InterjectRequestHandler handler);
    }

    public interface IResponseConverter
    {
        public void Convert(InterjectRequestHandler handler);
    }

    #endregion

    /// <summary>
    /// This is the container for the pipeline logic where Interject Requests are processed.<br/>
    /// </summary>
    /// <remarks>
    /// Use this method when all the parameters from the<see cref="InterjectRequest"/>can be
    /// processed with the same pipeline logic.
    /// </remarks>
    public class InterjectRequestHandler
    {
        #region Interface members

        /// <summary>
        /// An implementation of the <see cref="Classes.IParameterConverter"/> interface.
        /// </summary>
        public IParameterConverter IParameterConverter { get; set; } = new DefaultParameterConverter();

        /// <summary>
        /// An implementation of the <see cref="Classes.IDataConnection"/> interface.
        /// </summary>
        public IDataConnection IDataConnection { get; set; } = new DefaultDataConnection();

        /// <summary>
        /// An implementation of the <see cref="Classes.IDataConnectionAsync"/> interface.
        /// </summary>
        public IDataConnectionAsync IDataConnectionAsync { get; set; } = new DefaultDataConnectionAsync();

        /// <summary>
        /// An implementation of the <see cref="Classes.IResponseConverter"/> interface.
        /// </summary>
        public IResponseConverter IResponseConverter { get; set; } = new DefaultResponseConverter();

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

        private class DefaultResponseConverter : IResponseConverter
        {
            public void Convert(InterjectRequestHandler handler) { } // Do nothing
        }

        #endregion

        #region Data storage members

        /// <summary>
        /// A container for storing request parameter data after processing them in the
        /// <see cref="Classes.IParameterConverter"/>. This is useful for tracking the incomming
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
        /// is transformed into standard<see cref="IdsTable"/>data within the
        /// <see cref="InterjectResponse.ReturnedDataList"/> collection.
        /// </summary>
        public object ReturnData { get; set; }

        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="InterjectRequestHandler"/>.<br/>
        /// This stores the <see cref="InterjectRequest"/>, ensures the
        /// <see cref="InterjectRequest.RequestParameterList"/> is not null
        /// and instantiates a new <see cref="InterjectResponse"/> for the
        /// final return to the Interject add-in.
        /// </summary>
        /// <param name="request">The <see cref="InterjectRequest"/> from the Interject add-in.</param>
        public InterjectRequestHandler(InterjectRequest request)
        {
            this.IdsRequest = request;
            if (request.RequestParameterList == null) request.RequestParameterList = new();
            this.IdsResponse = new(request);
        }

        /// <summary>
        /// Performs the pipeline operations in the following order:
        /// <list>
        /// <item>1: <see cref="IParameterConverter.Convert(List{RequestParameter}, List{object})"/></item>
        /// <item>2: <see cref="IDataConnection.FetchData(InterjectRequestHandler)"/></item>
        /// <item>3: <see cref="IResponseConverter.Convert(InterjectRequestHandler)"/></item>
        /// </list>
        /// </summary>
        /// <returns><see cref="InterjectResponse"/></returns>
        public InterjectResponse ReturnResponse()
        {
            this.IParameterConverter.Convert(this.IdsRequest.RequestParameterList, this.ConvertedParameters);
            this.IDataConnection.FetchData(this);
            this.IResponseConverter.Convert(this);
            return this.PackagedResponse;
        }

        /// <summary>
        /// Performs the pipeline operations in the following order:
        /// <list>
        /// <item>1: <see cref="IParameterConverter.Convert(List{RequestParameter}, List{object})"/></item>
        /// <item>2: <see cref="IDataConnectionAsync.FetchDataAsync(InterjectRequestHandler)"/></item>
        /// <item>3: <see cref="IResponseConverter.Convert(InterjectRequestHandler)"/></item>
        /// </list>
        /// </summary>
        /// <returns><see cref="InterjectResponse"/></returns>
        public async Task<InterjectResponse> ReturnResponseAsync()
        {
            this.IParameterConverter.Convert(this.IdsRequest.RequestParameterList, this.ConvertedParameters);
            await this.IDataConnectionAsync.FetchDataAsync(this);
            this.IResponseConverter.Convert(this);
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
                    string json = JsonSerializer.Serialize(returnData.Data);
                    returnData.Data = json;
                });
                return this.IdsResponse;
            }
        }
    }
}