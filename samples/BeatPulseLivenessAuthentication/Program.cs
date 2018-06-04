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
                options.SetAlternatePath("health") //default hc
                    .EnableOutputCache(10)      // Can use CacheMode as second parameter
                    .SetTimeout(milliseconds: 1500) // default -1 infinitely
                    .EnableDetailedOutput(); //default false
            }).UseStartup<Startup>();
    }
}
