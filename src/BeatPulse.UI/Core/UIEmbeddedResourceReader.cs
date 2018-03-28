using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace BeatPulse.UI.Core
{
    public class UIEmbeddedResourcesReader : IUIResourceReader
    {
        private readonly Assembly _assembly;

        public UIEmbeddedResourcesReader(Assembly assembly)
        {
            _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
        }

        public IEnumerable<UIResource> GetUIResources
        {
            get
            {
                var embeddedResources = _assembly.GetManifestResourceNames();

                return ParseEmbeddedResources(embeddedResources);
            }
        }

        private IEnumerable<UIResource> ParseEmbeddedResources(string[] embeddedFiles)
        {
            const char SPLIT_SEPARATOR = '.';

            var resourceList = new List<UIResource>();

            foreach (var file in embeddedFiles)
            {
                var segments = file.Split(SPLIT_SEPARATOR);

                //temporal mapping

                var fileName = segments[segments.Length - 2];

                var extension = segments[segments.Length - 1];

                var contentStream = _assembly.GetManifestResourceStream(file);

                using (var reader = new StreamReader(contentStream))
                {

                    string result = reader.ReadToEnd();
                    
                    //temporal extension mapping
                    resourceList.Add(

                        UIResource.Create($"{fileName}.{extension}", result,

                        extension.ToLower().Contains("js") ? "text/javascript" : "text/html"));
                }
            }

            return resourceList;
        }
    }
}
