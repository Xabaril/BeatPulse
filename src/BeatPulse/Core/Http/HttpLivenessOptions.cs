using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace BeatPulse.Core.Http
{
    public class HttpLivenessOptions
    {
        public string Url { get; private set; }
        public string ContentCheck { get; private set; }
        public int? StatusCodeCheck { get; set; }
        public int? TimeOut { get; private set; }
        public HttpMethod Method { get; private set; } = HttpMethod.Get;
        
        public HttpLivenessOptions WithUrl(string url)
        {
            Url = url;
            return this;
        }
        /// <summary>
        /// Configure request timeout for the current http liveness
        /// </summary>
        /// <param name="timeout">Timeout in seconds</param>
        /// <returns></returns>
        public HttpLivenessOptions WithTimeout(int timeout)
        {
            TimeOut = timeout;
            return this;
        }

        public HttpLivenessOptions WithContentCheck(string contentToCheck)
        {
            ContentCheck = contentToCheck;
            return this;
        }

        public HttpLivenessOptions WithMethod(HttpMethod method)
        {
            Method = method;
            return this;
        }

        public HttpLivenessOptions WithStatusCode(int statusCode)
        {
            StatusCodeCheck = statusCode;
            return this;
        }
        
    }
}
