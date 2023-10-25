using System;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Interject.DataApi
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
        /// <param name="opt">
        /// Optional:
        /// <br/>
        /// If left out, the endpoint returns serialized <see cref="ApplicationOptions"/>
        /// <br/>
        /// If inclued, the options available are:
        /// <list type="bullet">
        /// <item>name, </item>
        /// <item>version, </item>
        /// <item>framework</item>
        /// </list>
        /// </param>
        /// <returns></returns>
        [HttpGet("Options")]
        public string GetInfo([FromQuery] string opt = null)
        {
            var result = string.Empty;
            if (string.IsNullOrEmpty(opt))
            {
                result = JsonSerializer.Serialize(_options);
            }
            else
            {
                try
                {
                    foreach (PropertyInfo prop in _options.GetType().GetProperties())
                    {
                        if (prop.Name.Equals(opt, StringComparison.OrdinalIgnoreCase))
                        {
                            result = prop.GetValue(_options)?.ToString();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    result = $"error: {e.Message}";
                }
            }
            return result;
        }
    }
}