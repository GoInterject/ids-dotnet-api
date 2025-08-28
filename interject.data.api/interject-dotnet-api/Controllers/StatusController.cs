using System;
using System.Reflection;
using System.Text.Json;
using System.Linq;
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
 
        /// <summary>
        /// Echo selected request info (method, path, normalized headers) for debugging.
        /// </summary>
        [HttpGet("headers")]
        public IActionResult GetHeaders() => Ok(CollectHeaders());
 
        /// <summary>
        /// Echo selected request info (method, path, normalized headers) for debugging.
        /// </summary>
        [HttpPost("headers")]
        public IActionResult PostHeaders() => Ok(CollectHeaders());

        private object CollectHeaders()
        {
            var headers = Request.Headers.ToDictionary(
                kv => kv.Key,
                kv => SanitizeHeader(kv.Key, string.Join(", ", kv.Value)),
                StringComparer.OrdinalIgnoreCase
                );

            return new
            {
                method = Request.Method,
                path = Request.Path.ToString(),
                headers
            };
        }
        private static string SanitizeHeader(string name, string value)
        {
            if (name.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(value)) return value ?? string.Empty;
                var parts = value.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                var scheme = parts.Length > 0 ? parts[0] : "Bearer";
                var token = parts.Length > 1 ? parts[1] : string.Empty;
                if (token.Length <= 10) return $"{scheme} ***";
                return $"{scheme} {token.Substring(0, 4)}…{token.Substring(token.Length - 4)}";
            }
            return value;
        }
    }
}