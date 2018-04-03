using System;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse.Core.Http
{
    public class HttpLivenessErrors
    {
        public const string INVALID_STATUS_CODE = "The configured status code to check does not match response";
        public const string INVALID_RESPONSE_CONTENT = "The configured content to check does not match response";
    }
}
