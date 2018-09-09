using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BeatPulseLivenessAuthentication
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
                    .ConfigureDetailedOutput(detailedOutput: true, includeExceptionMessages: true); //default false
            }).UseStartup<Startup>();
    }
}
