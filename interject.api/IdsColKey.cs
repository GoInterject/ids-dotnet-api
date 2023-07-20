using System.Collections.Generic;
using System.Text;

namespace Interject.Api
{
    public class IdsColKey
    {
        public int Order { get; set; } // The order of this ColKey in the list
        public int Column { get; set; } // The column number index of this item from Excel Report
        public string Value { get; set; } // The value of this ColKey
        public string Name { get; set; } // The name of this ColKey
        public Dictionary<string, string> Json { get; set; }

        /// <summary>
        /// Returns the string representation of this item in XML form
        /// </summary>
        /// <returns></returns>
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