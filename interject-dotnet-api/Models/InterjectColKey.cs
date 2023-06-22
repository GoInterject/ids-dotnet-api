using System.Collections.Generic;
using System.Text;
using Interject.Classes;

namespace Interject.Models
{
    public class InterjectColKey
    {
        public int Order { get; set; }
        public int Column { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Json { get; set; }

        public string ToXML()
        {
            StringBuilder sb = new();
            sb.Append("<Val");
            sb.Append($" Order:\"{Order}\"");
            sb.Append($" Column:\"{Column}\"");
            sb.Append($" Value:\"{Value}\"");
            sb.Append($" Name:\"{Name}\"");
            sb.Append($" JSON:\"{Json.ToJson()}\"/>");
            return sb.ToString();
        }
    }
}