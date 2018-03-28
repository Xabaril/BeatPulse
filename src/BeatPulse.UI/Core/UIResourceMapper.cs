using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace BeatPulse.UI.Core
{
    public class UIResourcesMapper
    {
        private readonly IUIResourceReader _reader;

        public UIResourcesMapper(IUIResourceReader reader)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public void Map(IApplicationBuilder app, string sufix)
        {
            var resources = _reader.GetUIResources;

            //map all resources
            
            foreach (var resource in resources)
            {
                app.Map($"{sufix}/{resource.FileName}", appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.ContentType = resource.ContentType;

                        await context.Response.WriteAsync(resource.Content);
                    });
                });
            }

            //map default sufix to index.html

            app.Map(sufix, appBuilder =>
            {
                appBuilder.Run(context =>
                {
                    context.Response.Redirect($"{sufix}/index.html", permanent: true);

                    return Task.CompletedTask;
                });
            });
        }
    }
}
