using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using BeatPulse;

namespace BeatPulseConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder();
            builder.UseBeatPulse(opt =>
           {
               opt.Setup(ctx =>
               {
                   ctx.AddSqlServer("my-db");
               });
           });
            await builder.RunConsoleAsync();
        }
    }
}
