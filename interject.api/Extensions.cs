using System.Collections.Generic;
using System.Text;

namespace Interject.Api
{
    public static class InterjectExtensions
    {
        /// <summary>
        /// Converts a Dictionary pair of strings to a Json formatted string
        /// </summary>
        /// <param name="dict"></param>
        /// <returns>Json formatted string</returns>
        public static string ToJson(this Dictionary<string, string> dict)
        {
            StringBuilder sb = new();
            sb.Append("{");
            var i = dict.Count;
            foreach (KeyValuePair<string, string> pair in dict)
            {
                i--;
                sb.Append($"\"{pair.Key}\":\"{pair.Value}\"");
                if (i > 0) sb.Append(",");
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}