using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace BeatPulseTrackers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
           WebHost.CreateDefaultBuilder(args)
               .UseApplicationInsights()
               .UseBeatPulse(setup =>
               {
                   setup.ConfigureDetailedOutput(detailedOutput: true, includeExceptionMessages: true)
                       .ConfigureTimeout(1500);
               }).UseStartup<Startup>();
    }
}
