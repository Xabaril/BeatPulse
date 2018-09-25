using System;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse.UI.Middleware
{
    public class ApiOptions
    {
        public string BeatPulseApiPath { get; set; } = "/beatpulse-api";
        public string BeatPulseUIPath { get; set; } = "/beatpulse-ui";
        public string BeatPulseWebHooksPath { get; set; } = "/beatpulse-webhooks";
    }
}
