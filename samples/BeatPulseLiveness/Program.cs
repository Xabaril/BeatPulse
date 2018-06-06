using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace BeatPulseLiveness
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseBeatPulse(options =>
            {
                options.ConfigurePath("health") //default hc
                    .ConfigureOutputCache(10)      // Can use CacheMode as second parameter
                    .ConfigureTimeout(milliseconds: 1500) // default -1 infinitely
                    .ConfigureDetailedOutput(); //default false
            }).UseStartup<Startup>();
    }
}
