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
                        //.ConfigurePort(65400)  //use only with this sample is executed on commandname Project not on IIS
                        .ConfigureOutputCache(10)      // Can use CacheMode as second parameter
                        .ConfigureTimeout(milliseconds: 1500) // default -1 infinitely
                        .ConfigureDetailedOutput(includeExceptionMessages: true); //default false
                }).UseStartup<Startup>();
    }
}
