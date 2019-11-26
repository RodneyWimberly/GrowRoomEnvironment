using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using ZNetCS.AspNetCore.Logging.EntityFrameworkCore;

namespace GrowRoomEnvironment.DataAccess.Models
{
    public class ExtendedLog : Log
    {
        public ExtendedLog(IHttpContextAccessor accessor)
        {
            if (accessor != null && accessor.HttpContext != null)
            {
                string browser = accessor.HttpContext.Request.Headers["User-Agent"];
                if (!string.IsNullOrEmpty(browser) && (browser.Length > 255))
                {
                    browser = browser.Substring(0, 255);
                }

                Browser = browser;
                Host = accessor.HttpContext.Connection?.RemoteIpAddress?.ToString();
                User = accessor.HttpContext.User?.Identity?.Name;
                Path = accessor.HttpContext.Request.Path;
            }
        }

        protected ExtendedLog()
        {
        }

        public string Browser { get; set; }
        public string Host { get; set; }
        public string Path { get; set; }
        public string User { get; set; }
    }
}
