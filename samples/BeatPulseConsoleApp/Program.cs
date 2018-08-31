using BeatPulse;
using BeatPulse.Hosted;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace BeatPulseConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder();
            builder.UseBeatPulse(opt =>
           {
               opt.Configure(bp =>
               {
                   bp.ConfigureDetailedOutput(true);
                   bp.ConfigurePath("hc");
                   // bp.UseOutputFormatter<JsonOutputFormatter>(sp => new JsonOutputFormatter().Configure(json => { }));
                   bp.UseOutputFormatter<JsonOutputFormatter>();
               });

               opt.Setup(ctx =>
               {
                   ctx.AddSqlServer("my-db");
               });
           });
            await builder.RunConsoleAsync();
        }
    }
}
