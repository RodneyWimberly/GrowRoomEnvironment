using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrowRoomEnvironment.Core.Logging
{
    public class LoggerState
    {
        private HttpContext _httpContext;
        public LoggerState(IHttpContextAccessor accessor)
        {
            _httpContext = accessor?.HttpContext;
        }

        public Dictionary<string, object> State
        {
            get
            {
                if (_httpContext == null)
                    return new Dictionary<string, object>();
                else
                    return GetLoggerState(_httpContext);
            }
        }

        private Dictionary<string, object> GetLoggerState(HttpContext context)
        {
            Dictionary<string, object> loggerState = new Dictionary<string, object>
            {
                { "url", context.Request?.Path.Value },
                { "method", context.Request?.Method },
                { "statuscode", context.Response.StatusCode },
                { "user", context.User?.Identity?.Name },
                { "servervariables", ServerVariables(context) },
                { "cookies", Cookies(context) },
                { "form", Form(context) },
                { "querystring", QueryString(context) },
            };
            return loggerState;
        }

        private Dictionary<string, string> QueryString(HttpContext context)
        {
            return context.Request?.Query?.Keys.ToDictionary(k => k, k => context.Request.Query[k].ToString());
        }

        private Dictionary<string, string> Form(HttpContext context)
        {
            try
            {
                return context.Request?.Form?.Keys.ToDictionary(k => k, k => context.Request.Form[k].ToString());
            }
            catch (InvalidOperationException)
            {
                // Request not a form POST or similar
            }

            return new Dictionary<string, string>();
        }

        private Dictionary<string, string> Cookies(HttpContext context)
        {
            return context.Request?.Cookies?.Keys.ToDictionary(k => k, k => context.Request.Cookies[k].ToString());
        }

        private Dictionary<string, string> ServerVariables(HttpContext context)
        {
            return context.Request?.Headers?.Keys.ToDictionary(k => k, k => context.Request.Headers[k].ToString());
        }

    }
}
