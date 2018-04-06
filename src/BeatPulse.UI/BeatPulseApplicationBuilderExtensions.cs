using BeatPulse.UI.Core;
using Microsoft.AspNetCore.Builder;

namespace BeatPulse.UI
{
    public static class BeatPulseApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseBeatPulseUI(this IApplicationBuilder app, string uiPath = "/beatpulse-ui", string apiPath = "/beatpulse-api")
        {
            var embeddedResourcesAssembly = typeof(UIResource).Assembly;

            //register api middleware
            app.Map(pathMatch: apiPath, appBuilder => appBuilder.UseMiddleware<UIApiEndpointMiddleware>());

            //register ui middleware for embebbed resources
            new UIResourcesMapper(
                new UIEmbeddedResourcesReader(embeddedResourcesAssembly)).Map(app, suffix: uiPath);

            return app;
        }
    }
}
