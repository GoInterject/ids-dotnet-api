using System.Collections.Generic;

namespace Interject.Models
{
    /// <summary>
    /// All requests to the Data Engine API must use the<see cref="InterjectRequestDTO"/>class as the body of the request.
    /// </summary>
    public class InterjectRequestDTO
    {
        public string DataPortalName { get; set; }
        public List<RequestParameter> RequestParameterList { get; set; }
        public PassThroughCommand PassThroughCommand { get; set; }

        public InterjectRequestDTO() { }
    }
}