using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Interject.Classes
{
    /// <summary>
    /// This is a container for the pipeline logic where Interject Requests are processed.<br/>
    /// </summary>
    public class InterjectRequestHandler
    {
        /// <summary>
        /// A container for storing request parameter data after processing them in the
        /// <see cref="IParameterConverter"/>. This is useful for tracking the incomming
        /// request throughout the pipeline.
        /// </summary>
        public List<object> ConvertedParameters { get; set; } = new();

        /// <summary>
        /// A backward compatiblity feature for supporting a passthrough of a connection string
        /// name for lookup in the appsettings.json or the connection string itself.
        /// </summary>
        public string ConnectionString { get; set; }

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

        /// <summary>
        /// Provided by dependancy injection during the application startup. This is coming
        /// from the appSettings.json "Connections" collection property.
        /// </summary>
        private List<ConnectionDescriptor> _connectionStrings;

        /// <summary>
        /// Create an instance of <see cref="InterjectRequestHandler"/>
        /// </summary>
        /// <param name="connectionStringOptions"></param>
        public InterjectRequestHandler(ConnectionStringOptions connectionStringOptions)
        {
            if (connectionStringOptions == null)
            {
                _connectionStrings = new();
            }
            else if (connectionStringOptions.ConnectionStrings == null)
            {
                _connectionStrings = new();
            }
            else
            {
                _connectionStrings = connectionStringOptions.ConnectionStrings;
            }
        }

        /// <summary>
        /// The first step in the request pipeline. This validates the incomming request and
        /// initializes the output response object.
        /// </summary>
        /// <param name="request">The request passed from the Interject Addin to the Api endpoint.</param>
        public void Init(InterjectRequest request)
        {
            this.IdsRequest = request;
            if (request.RequestParameterList == null) request.RequestParameterList = new();
            this.IdsResponse = new InterjectResponse(request);
            ResolveConnectionString();
        }

        private void ResolveConnectionString()
        {
            if (IdsRequest.PassThroughCommand == null) IdsRequest.PassThroughCommand = new();
            var conStrDesc = _connectionStrings.FirstOrDefault(cs => cs.Name == this.IdsRequest.PassThroughCommand.ConnectionStringName);

            if (conStrDesc == null)
            {
                // IdsRequest.PassThroughCommand.ConnectionStringName 
                // may be the connection string itself.
                this.ConnectionString = this.IdsRequest.PassThroughCommand.ConnectionStringName;
            }
            else
            {
                this.ConnectionString = conStrDesc.ConnectionString;
            }
        }

        /// <summary>
        /// Converts the request parameters into the type needed for the data handler's data integration.
        /// </summary>
        /// <param name="converter">The instance of the <see cref="IParameterConverter"/> derived class used to convert
        /// <see cref="RequestParameter"/> to the type required for the data connection type to consume.</param>
        public void ConvertParameters(IParameterConverter converter = null)
        {
            if (converter == null)
            {
                this.IdsRequest.RequestParameterList.ForEach((param) =>
                {
                    this.ConvertedParameters.Add(param);
                });
            }
            else
            {
                converter.Convert(this);
            }
        }

        /// <summary>
        /// An async option for collecting the data. This is intended to pass the collected data to the
        /// <see cref="InterjectRequestHandler.ReturnData"/> member for later conversion.
        /// </summary>
        /// <param name="dataConnection">The instance of the <see cref="IDataConnection"/> derived class used to
        /// collect the data from the source.</param>
        public async Task FetchDataAsync(IDataConnection dataConnection)
        {
            await dataConnection.FetchDataAsync(this);
        }

        /// <summary>
        /// A synchronous option for collecting the data. This is intended to pass the collected data to the
        /// <see cref="InterjectRequestHandler.ReturnData"/>property for later conversion.
        /// </summary>
        /// <param name="dataConnection">The instance of the <see cref="IDataConnection"/> derived class used to
        /// collect the data from the source.</param>
        public void FetchData(IDataConnection dataConnection)
        {
            dataConnection.FetchData(this);
        }

        /// <summary>
        /// The final phase of the request pipeline. The data stored in the
        /// <see cref="InterjectRequestHandler.ReturnData"/>property from one of the two Fetch methods is now
        /// converted into the standard<see cref="ReturnedData"/>class and added to the 
        /// <see cref="InterjectResponse.ReturnedDataList"/>collection of the 
        /// <see cref="InterjectRequestHandler.IdsResponse"/> property.
        /// </summary>
        /// <param name="converter"></param>
        /// <exception cref="UserException"></exception>
        public void ConvertResponseData(IResponseConverter converter)
        {
            if (this.ReturnData == null)
            {
                throw new UserException("The returned data was null. Be sure InterjectRequestHandler.FetchData was called and that the IDataConnection.FetchDataAsync passed in returns data.");
            }
            else
            {
                converter.Convert(this);
            }
        }

        /// <summary>
        /// GETTER: Performs final serialization required for the addin to consume the response.
        /// </summary>
        public InterjectResponse PackagedResponse
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