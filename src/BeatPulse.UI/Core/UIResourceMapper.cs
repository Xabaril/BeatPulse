using BeatPulse.UI.Core.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace BeatPulse.UI.Core
{
    public class UIResourcesMapper
    {
        private readonly IUIResourcesReader _reader;

        public UIResourcesMapper(IUIResourcesReader reader)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public void Map(IApplicationBuilder app, string suffix)
        {
            var resources = _reader.GetUIResources;

            //map all resources

            foreach (var resource in resources)
            {
                app.Map($"{suffix}/{resource.FileName}", appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.ContentType = resource.ContentType;

                        await context.Response.WriteAsync(resource.Content);
                    });
                });
            }

            //map default sufix to index.html

            app.Map(suffix, appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    if (await UIAuthorizationHelper.IsAuthorizedAsync(context))
                    {                        
                        context.Response.Redirect($"{suffix}/index.html", permanent: true);
                    }
                    else
                    {
                        context.Response.StatusCode = 401;
                    }

                });
            });
        }
    }
}
