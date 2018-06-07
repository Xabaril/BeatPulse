using System;
using BeatPulse.UI.Core;
using BeatPulse.UI.Middleware;
using Microsoft.AspNetCore.Builder;

namespace BeatPulse.UI
{
    public static class BeatPulseApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseBeatPulseUI(this IApplicationBuilder app, Action<ApiOptions> setupApiOptions)
        {
            var apiOptions = new ApiOptions();
            setupApiOptions(apiOptions);

            return ConfigurePipeline(app, apiOptions);
        }

        [Obsolete("This method is obsolete, use overload with ApiOptions instead")]
        public static IApplicationBuilder UseBeatPulseUI(this IApplicationBuilder app, string uiPath = "/beatpulse-ui", string apiPath = "/beatpulse-api", string webHooksPath = "/beatpulse-webhooks")
        {
            return ConfigurePipeline(app, new ApiOptions() {
                BeatPulseApiPath = apiPath,
                BeatPulseUIPath = uiPath,
                BeatPulseWebHooksPath = webHooksPath
            });
        }

        private static IApplicationBuilder ConfigurePipeline(IApplicationBuilder app, ApiOptions apiOptions)
        {
            var embeddedResourcesAssembly = typeof(UIResource).Assembly;

            app.Map(apiOptions.BeatPulseApiPath, appBuilder => appBuilder.UseMiddleware<UIApiEndpointMiddleware>());
            app.Map(apiOptions.BeatPulseWebHooksPath, appBuilder => appBuilder.UseMiddleware<UIWebHooksApiMiddleware>());

            new UIResourcesMapper(
                new UIEmbeddedResourcesReader(embeddedResourcesAssembly)).Map(app, apiOptions);

            return app;
        }
    }
}
