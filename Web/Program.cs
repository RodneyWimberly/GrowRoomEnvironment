using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

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
               // .ConfigureLogging((hostingContext, builder) => builder.AddFile("Logs/GrowRoomEnvironment-{Date}.txt"))
                .UseStartup<Startup>()
                .UseUrls("http://*:5001/");
    }
}
