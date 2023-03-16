using Interject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Interject.API
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RequestController : ControllerBase
    {
        private readonly ConnectionStringOptions _connectionStringOptions;
        public RequestController(ConnectionStringOptions options)
        {
            _connectionStringOptions = options;
        }

        /// <summary>
        /// The principal method of processing an action in the Interject Addin. E.G. Pull, Save, Drill
        /// </summary>
        /// <param name="interjectRequest">
        /// The <see cref="InterjectRequestDTO"/> object to process.
        /// </param>
        [HttpPost]
        [ProducesResponseType(typeof(InterjectResponseDTO), 200)]
        public async Task<InterjectResponseDTO> Post([FromBody] InterjectRequestDTO interjectRequest)
        {
            InterjectResponseDTO response = new(interjectRequest);
            return response;
        }
    }
}