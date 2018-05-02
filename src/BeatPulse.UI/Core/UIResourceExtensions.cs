using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeatPulse.UI.Core
{
    static class UIResourceExtensions
    {
        /// <summary>
        /// Returns a collection of embedded resources matching the specified content type 
        /// </summary>
        /// <param name="resources"></param>
        /// <param name="contentType">Target content type</param>
        /// <returns></returns>
        public static IEnumerable<UIResource> WithContentType(this IEnumerable<UIResource> resources, string contentType)
        {
            return resources.Where(r => r.ContentType == contentType);
        }           
        /// <summary>
        /// Returns embedded UI main html resource with target ApiPath configured for request polling
        /// </summary>
        /// <param name="resources"></param>
        /// <param name="apiPath"></param>
        /// <returns></returns>
        public static UIResource GetMainUI(this IEnumerable<UIResource> resources, string apiPath)
        {
            var resource = resources.WithContentType(ContentType.HTML).FirstOrDefault(r => r.FileName == BeatPulseUIKeys.BEATPULSEUI_MAIN_UI_RESOURCE);
            resource.Content = resource.Content.Replace(BeatPulseUIKeys.BEATPULSEUI_MAIN_UI_API_TARGET, apiPath);
            return resource;
        }
    }
}
