using System;
using System.Collections.Generic;
using System.Linq;

namespace Interject.Api
{
    /// <summary>
    /// All requests to the Data API must use the <see cref="InterjectRequest"/>class as the body of the request.
    /// </summary>
    public class InterjectRequest
    {
        /// <summary>
        /// The name of the Data Portal.
        /// </summary>
        public string DataPortalName { get; set; }

        /// <summary>
        /// The collection of Request Parameters configured in the Data Portal.
        /// </summary>
        /// <remarks>
        /// This includes the System Parameters.
        /// </remarks>
        public List<RequestParameter> RequestParameterList { get; set; } = new();

        /// <summary>
        /// The command arguments configured in the Data Portal.
        /// </summary>
        public PassThroughCommand PassThroughCommand { get; set; } = new();

        public Dictionary<string, string> SupplementalData { get; set; }

        public InterjectRequest() { }

        private InterjectRequestContext _requestContext { get; set; }

        /// <summary>
        /// The <see cref="InterjectRequestContext"/>.
        /// </summary>
        /// <remarks>
        /// Requires the Data Portal System Parameter: Interject_RequestContext
        /// </remarks>
        public InterjectRequestContext GetRequestContext()
        {
            if (_requestContext == null)
            {
                _requestContext = GetParameterValue<InterjectRequestContext>("@Interject_RequestContext");
            }
            return _requestContext;
        }

        /// <summary>
        /// The collection of <see cref="InterjectColDefItem"/> configured in the report.
        /// </summary>
        /// <remarks>
        /// Requires the Data Portal System Parameter: Interject_RequestContext
        /// </remarks>
        public List<InterjectColDefItem> GetColDefItems()
        {
            return this.GetRequestContext()?.ColDefItems;
        }

        /// <summary>
        /// The collection of <see cref="InterjectRowDefItem"/> configured in the report.
        /// </summary>
        /// <remarks>
        /// Requires the Data Portal System Parameter: Interject_RequestContext
        /// </remarks>
        public List<InterjectRowDefItem> GetRowDefItems()
        {
            return this.GetRequestContext()?.RowDefItems;
        }

        /// <summary>
        /// The <see cref="IdsUserContext"/>.
        /// </summary>
        /// <remarks>
        /// Requires the Data Portal System Parameter: Interject_RequestContext
        /// </remarks>
        public IdsUserContext UserContext()
        {
            return this.GetRequestContext()?.UserContext;
        }

        private IdsTable _xmlDataToSave { get; set; }

        /// <summary>
        /// The <see cref="IdsTable"/> storing the data coming from the report.
        /// </summary>
        /// <remarks>
        /// NOTE: If your Data Portal includes the System Parameter: Interject_RequestContext
        /// the Interject_XmlDataToSave will be empty, the data will be in the 
        /// RequestContext.XmlDataToSave instead.
        /// </remarks>
        public IdsTable GetXmlDataToSave()
        {
            if (_xmlDataToSave != null) return _xmlDataToSave;
            if (this.GetRequestContext()?.XmlDataToSave != null)
            {
                return this.GetRequestContext()?.XmlDataToSave;
            }
            else
            {
                _xmlDataToSave = GetParameterValue<IdsTable>("@Interject_XmlDataToSave");
            }
            return _xmlDataToSave;
        }

        /// <summary>
        /// Looks for a parameter in the <see cref="InterjectRequest.RequestParameterList"/>
        /// matching the name.
        /// </summary>
        /// <remarks>
        /// NOTE: Interject prepends the prefix '@' to all parameters. This method will
        /// account for that by prepending '@' to the name parameter as needed.
        /// </remarks>
        /// <param name="name">
        /// The value of the<see cref="RequestParameter.Name"/>property.
        /// </param>
        /// <exception cref="Exception">If a matching parameter is not found.</exception>
        public RequestParameter GetParameter(string name)
        {
            if (!name.StartsWith('@')) name = $"@{name}";
            var result = RequestParameterList.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (result == null) throw new Exception($"Request Parameter '{name}' not found.");
            return result;
        }

        /// <summary>
        /// Searches the<see cref="InterjectRequest.RequestParameterList"/>
        /// collection for a <see cref="RequestParameter"/> with a matching name.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the return value.<br/>
        /// Options:
        /// <list type="bullet">
        /// <item>int</item>
        /// <item>float</item>
        /// <item>string</item>
        /// <item>InterjectTable</item>
        /// <item>InterjectRequestContext</item>
        /// </list>
        /// </typeparam>
        /// <param name="name">
        /// The <see cref="RequestParameter.Name"/> to search for.
        /// </param>
        /// <returns>The <see cref="RequestParameter.InputValue"/> cast to the type in the T parameter.
        /// </returns>
        /// <exception cref="Exception"></exception>
        public T GetParameterValue<T>(string name)
        {
            RequestParameter param = GetParameter(name);
            if (string.IsNullOrEmpty(param.InputValue)) return default(T);
            object result = default;

            try
            {
                if (typeof(T) == typeof(string))
                {
                    result = param.InputValue;
                }
                else if (typeof(T) == typeof(int))
                {
                    result = int.Parse(param.InputValue);
                }
                else if (typeof(T) == typeof(int?))
                {
                    result = int.Parse(param.InputValue);
                }
                else if (typeof(T) == typeof(float))
                {
                    result = float.Parse(param.InputValue);
                }
                else if (typeof(T) == typeof(float?))
                {
                    result = float.Parse(param.InputValue);
                }
                else if (typeof(T) == typeof(IdsTable))
                {
                    result = ParameterParser.ParseXmlDataToSave(param.InputValue);
                }
                else if (typeof(T) == typeof(InterjectRequestContext))
                {
                    result = ParameterParser.ParseRequestContext(param.InputValue);
                }
                else
                {
                    throw new Exception($"Cannot convert paramter values to type: {typeof(T).Name}");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error parsing parameter '{name}' to type '{typeof(T).Name}'.", e);
            }

            return (T)result;
        }


    }
}
