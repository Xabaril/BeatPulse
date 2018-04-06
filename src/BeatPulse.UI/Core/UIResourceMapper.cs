using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;

namespace BeatPulse.UI.Core
{
    class UIResourcesMapper
    {
        private readonly IUIResourcesReader _reader;

        public UIResourcesMapper(IUIResourcesReader reader)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public void Map(IApplicationBuilder app, string suffix)
        {
            var resources = _reader.GetUIResources;

            // map all resources on assets folders to 
            // request uri by resource name

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

            //redirect default suffix into index.html or return forbidden if is not authorized

            app.Map(suffix, appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    if (await context.IsAuthorizedAsync())
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
