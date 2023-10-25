using System.Collections.Generic;
using System.Text;

namespace Interject.Api
{
    /// <summary>
    /// A ColKey represents a column in the <see cref="InterjectRowDefItem"/> range
    /// </summary>
    public class IdsColKey
    {
        /// <summary>
        /// The order of this ColKey in a list of IdsColKeys
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// The column number index of this item from Excel Report
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// The value of this ColKey
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The name of this ColKey
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A Dictionary pair of strings representing the Json object of the Col
        /// </summary>
        public Dictionary<string, string> Json { get; set; }

        /// <returns>The string representation of this item in XML form</returns>
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