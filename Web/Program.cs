using GrowRoomEnvironment.DataAccess;
using GrowRoomEnvironment.DataAccess.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using ZNetCS.AspNetCore.Logging.EntityFrameworkCore;

namespace GrowRoomEnvironment.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
              .ConfigureLogging((hostingContext, logging) =>
              {
                  logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                  logging.AddEntityFramework<ApplicationDbContext, ExtendedLog>();
              })
            .UseStartup<Startup>()
            .UseUrls("http://*:5001/");
    }
}
