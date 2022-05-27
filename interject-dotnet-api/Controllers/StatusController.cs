using Microsoft.AspNetCore.Mvc;

namespace Interject.API
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly ApplicationOptions _options;
        public StatusController(ApplicationOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Check to see if the API is running and responding.
        /// </summary>
        /// <returns>true</returns>
        [HttpGet]
        public bool GetStatus()
        {
            return true;
        }

        /// <summary>
        /// Provides a method to get application information.
        /// </summary>
        /// <param name="opt">[name|version]</param>
        /// <returns></returns>
        [HttpGet("info")]
        public ApplicationOptions GetInfo([FromRoute] string opt = null)
        {
            return _options;
        }
    }
}