using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interject.Classes
{
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
}