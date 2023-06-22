using System.Collections.Generic;
using System.Text;

namespace Interject.Classes
{
    public static class InterjectExtensions
    {
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