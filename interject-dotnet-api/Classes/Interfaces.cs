using System.Threading.Tasks;

namespace Interject.Classes
{
    /// <summary>
    /// Consumed by the <see cref="InterjectRequestHandler.ConvertedParameters"/>
    /// during the fisrt phase of the request pipeline.
    /// <remarks>
    /// Intended to process each of the <see cref="RequestParameter"/> in
    /// the <see cref="InterjectRequest.RequestParameterList"/>collection of the
    /// incoming request.<br/>The resultant objects can be stored in the
    /// <see cref="InterjectRequestHandler.ConvertedParameters"/>collection for
    /// later access during the rest of the request pipeline.
    /// </remarks>
    /// </summary>
    public interface IParameterConverter
    {
        public void Convert(InterjectRequestHandler handler);
    }

    public interface IDataConnection
    {
        public void FetchData(InterjectRequestHandler handler);

        public Task FetchDataAsync(InterjectRequestHandler handler);
    }

    public interface IResponseConverter
    {
        public void Convert(InterjectRequestHandler handler);
    }
}