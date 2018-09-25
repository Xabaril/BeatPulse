﻿using System;
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
        public static IApplicationBuilder UseBeatPulseUI(this IApplicationBuilder app)
        {
            return ConfigurePipeline(app, new ApiOptions());
        }

        private static IApplicationBuilder ConfigurePipeline(IApplicationBuilder app, ApiOptions apiOptions)
        {
            EnsureValidApiOptions(apiOptions);

            var embeddedResourcesAssembly = typeof(UIResource).Assembly;

            app.Map(apiOptions.BeatPulseApiPath, appBuilder => appBuilder.UseMiddleware<UIApiEndpointMiddleware>());
            app.Map(apiOptions.BeatPulseWebHooksPath, appBuilder => appBuilder.UseMiddleware<UIWebHooksApiMiddleware>());

            new UIResourcesMapper(
                new UIEmbeddedResourcesReader(embeddedResourcesAssembly)).Map(app, apiOptions);

            return app;
        }

        private static void EnsureValidApiOptions(ApiOptions apiOptions)
        {
            Action<string, string> ensureValidPath = (string path, string argument) =>
             {
                 if (string.IsNullOrEmpty(path) || !path.StartsWith("/"))
                 {
                     throw new ArgumentNullException(argument);
                 }
             };

            ensureValidPath(apiOptions.BeatPulseApiPath, nameof(ApiOptions.BeatPulseApiPath));
            ensureValidPath(apiOptions.BeatPulseUIPath, nameof(ApiOptions.BeatPulseUIPath));
            ensureValidPath(apiOptions.BeatPulseWebHooksPath, nameof(ApiOptions.BeatPulseWebHooksPath));
        }
    }
}
