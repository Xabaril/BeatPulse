using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace BeatPulse.Hosted
{
    public class JsonOutputFormatterOptions
    {
        public Encoding Encoding { get; set; }
        public JsonSerializerSettings JsonSettings { get; }

        public JsonOutputFormatterOptions()
        {
            Encoding = Encoding.UTF8;
            JsonSettings = new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() };
        }
    }
}
