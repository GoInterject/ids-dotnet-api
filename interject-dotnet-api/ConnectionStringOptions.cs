using System.Collections.Generic;

namespace Interject
{
    public class ConnectionStringOptions
    {
        public const string Connections = "Connections";

        public List<ConnectionDescriptor> ConnectionStrings { get; set; } = new();
    }

    public class ConnectionDescriptor
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
    }
}