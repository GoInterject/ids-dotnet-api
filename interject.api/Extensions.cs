// Copyright 2024 Interject Data Systems

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.


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