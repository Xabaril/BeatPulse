using BeatPulse.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace BeatPulse.Hosted
{
    public class JsonOutputFormatter : IHostedBeatpulseOutputFormatter
    {

        private JsonOutputFormatterOptions _options;

        public JsonOutputFormatter()
        {
            _options = new JsonOutputFormatterOptions();                
        }


        public JsonOutputFormatter Configure(Action<JsonOutputFormatterOptions> confAction)
        {
            confAction.Invoke(_options);
            return this;
        }

        byte[] IHostedBeatpulseOutputFormatter.Serialize(OutputLivenessMessage output, BeatPulseOptions beatPulseOptions)
        {
            var content = beatPulseOptions.DetailedOutput ? JsonConvert.SerializeObject(output, _options.JsonSettings)
                : Enum.GetName(typeof(HttpStatusCode), output.Code);

            return _options.Encoding.GetBytes(content);
        }


    }


}
