using BeatPulse.UI.Core;
using Microsoft.AspNetCore.Builder;

namespace BeatPulse.UI
{
    public static class BeatPulseApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseBeatPulseUI(this IApplicationBuilder app, string path = "/beatpulse-ui", string apiEndpoint = "/beatpulse-api")
        {
            var embeddedResourcesAssembly = typeof(UIResource).Assembly;

            app.Map($"{apiEndpoint}", appBuilder => appBuilder.UseMiddleware<UIApiEndpointMiddleware>());
            new UIResourcesMapper(
                new UIEmbeddedResourcesReader(embeddedResourcesAssembly)).Map(app, suffix: path);           

            return app;
        }
    }
}
