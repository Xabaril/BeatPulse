using BeatPulse.UI.Core;
using Microsoft.AspNetCore.Builder;

namespace BeatPulse.UI
{
    public static class BeatPulseApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseBeatPulseUI(this IApplicationBuilder app, string uiPath = "/beatpulse-ui", string apiPath = "/beatpulse-api")
        {
            var embeddedResourcesAssembly = typeof(UIResource).Assembly;

            app.Map(pathMatch: apiPath, configuration: appBuilder => appBuilder.UseMiddleware<UIApiEndpointMiddleware>());

            new UIResourcesMapper(
                new UIEmbeddedResourcesReader(embeddedResourcesAssembly)).Map(app, uiPath, apiPath);

            return app;
        }
    }
}
