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
               .UseBeatPulse(setup =>
               {
                   setup.ConfigureDetailedOutput()
                       .ConfigureTimeout(1500);
               }).UseStartup<Startup>();
    }
}
