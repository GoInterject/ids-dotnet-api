using System.Collections.Generic;

namespace Interject.DataApi.Config
{
    public class ConnectionStringOptions
    {
        public const string Connections = "Connections";

        public List<ConnectionDescriptor> ConnectionStrings { get; set; } = new();

        public ConnectionStringOptions() { }

        public ConnectionStringOptions(ConnectionStringOptions options)
        {
            ConnectionStrings = options.ConnectionStrings;
        }
    }
}