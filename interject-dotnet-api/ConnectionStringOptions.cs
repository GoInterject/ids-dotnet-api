using System.Collections.Generic;

namespace Interject
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

    public class ConnectionDescriptor
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
    }
}