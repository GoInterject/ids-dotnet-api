using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interject.Classes
{
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