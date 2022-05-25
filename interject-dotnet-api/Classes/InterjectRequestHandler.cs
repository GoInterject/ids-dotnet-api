using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interject.Classes
{
    /// <summary>
    /// This is a container for the pipeline logic where Interject Requests are
    /// processed.
    /// </summary>
    public class InterjectRequestHandler
    {
        public List<object> ConvertedParameters { get; set; } = new();
        public string ConnectionString { get; set; }
        public InterjectRequest IdsRequest { get; set; }
        public InterjectResponse IdsResponse { get; set; }
        public object ReturnData { get; set; }
        private List<ConnectionDescriptor> _connectionStrings;

        public InterjectRequestHandler(ConnectionStringOptions connectionStringOptions)
        {
            _connectionStrings = connectionStringOptions.ConnectionStrings;
        }

        /// <summary>
        /// The first step in the request pipeline. This validates the incomming request and
        /// initializes the output response object.
        /// </summary>
        /// <param name="request">The request passed from the Interject Addin to the Api endpoint.</param>
        public void Init(InterjectRequest request)
        {
            Validate(request);
            this.IdsRequest = request;
            this.IdsResponse = new InterjectResponse(request);
            ResolveConnectionString();
        }

        private void Validate(InterjectRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.PassThroughCommand == null) throw new ArgumentException("PassThroughCommand is required.");
            if (string.IsNullOrEmpty(request.PassThroughCommand.ConnectionStringName)) throw new ArgumentException("PassThroughCommand.ConnectionStringName is required.");
        }

        private void ResolveConnectionString()
        {
            var conStr = _connectionStrings.FirstOrDefault(cs => cs.Name == this.IdsRequest.PassThroughCommand.ConnectionStringName)?.ConnectionString;

            if (string.IsNullOrEmpty(conStr))
            {
                // IdsRequest.PassThroughCommand.ConnectionStringName 
                // may be the connection string itself.
                this.ConnectionString = this.IdsRequest.PassThroughCommand.ConnectionStringName;
            }
            else
            {
                this.ConnectionString = conStr;
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
                this.IdsRequest.RequestParameters.ForEach((param) =>
                {
                    this.ConvertedParameters.Add(param);
                });
            }
            else
            {
                converter.Convert(this);
            }
        }

        public async Task FetchDataAsync(IDataConnection dataConnection)
        {
            await dataConnection.FetchDataAsync(this);
        }

        public void ConvertResponseData(IResponseConverter converter)
        {
            if (this.ReturnData == null)
            {
                throw new Exception("The returned data was null. Be sure InterjectRequestHandler.FetchData was called and that the IDataConnection.FetchDataAsync passed in returns data.");
            }
            else
            {
                converter.Convert(this);
            }
        }
    }
}