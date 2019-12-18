using GrowRoomEnvironment.DataAccess;
using GrowRoomEnvironment.DataAccess.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO;
using ZNetCS.AspNetCore.Logging.EntityFrameworkCore;

namespace GrowRoomEnvironment.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddEntityFramework<ApplicationDbContext, ExtendedLog>();
                    logging.AddEventLog(configure =>
                    {
                        configure.LogName = "GrowRoomEnvironment";
                        configure.SourceName = "GrowRoomEnvironment";
                        configure.Filter = (string category, LogLevel level) =>
                        {
                            if (category.Contains("GrowRoomEnvironment") ||
                                category.Contains("Controller") ||
                                category.Contains("Repository"))
                                return true;
                            if (level > LogLevel.Information)
                                return true;
                            else
                                return false;
                        };
                    });
                    logging.AddEventSourceLogger();
                    logging.AddTraceSource(
                        new SourceSwitch("GrowRoomEnvironment", "GrowRoomEnvironment Event Trace Log") 
                        { 
                            Level = SourceLevels.All
                        },
                        new XmlWriterTraceListener("EventTraceLog.xml"));
                    logging.SetMinimumLevel(LogLevel.Trace);
                });
                webBuilder.UseStartup<Startup>();
                webBuilder.UseUrls("http://*:5000/", "https://*:5001/");
            });
    }
}
