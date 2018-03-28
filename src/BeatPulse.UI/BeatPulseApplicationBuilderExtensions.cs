using BeatPulse.UI.Core;
using Microsoft.AspNetCore.Builder;

namespace BeatPulse.UI
{
    public static class BeatPulseApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseBeatPulseUI(this IApplicationBuilder app, string path = "/beatpulse-ui")
        {
            var embeddedResourcesAssembly = typeof(UIResource).Assembly;

            new UIResourcesMapper(
                new UIEmbeddedResourcesReader(embeddedResourcesAssembly)).Map(app, suffix: path);

            return app;
        }
    }
}
