using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BeatPulse.Core.Http
{
    public class HttpLiveness : IBeatPulseLiveness
    {
        public string Name => nameof(HttpLiveness);

        public string DefaultPath => "httpcheck";

        private HttpLivenessOptions _httpOptions;
        private int _defaultTimeout = 5;

        public HttpLiveness(Action<HttpLivenessOptions> options)
        {
            options(_httpOptions);              
        }

        public HttpLiveness(HttpLivenessOptions options)
        {
            _httpOptions = options;
        }

        public async Task<(string, bool)> IsHealthy(HttpContext context, bool isDevelopment, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var client = new HttpClient())
                {                    
                    client.Timeout = TimeSpan.FromSeconds(_httpOptions.TimeOut ?? _defaultTimeout);
                    var requestMessage = new HttpRequestMessage(_httpOptions.Method, _httpOptions.Url);                                        
                    var response = await client.SendAsync(requestMessage);
                    await AssertResponse(response);
                    return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
                }
            }
            catch (Exception ex)
            {               
                return (ex.Message, false);
            }
        }

        private async Task AssertResponse(HttpResponseMessage response)
        {
            if (_httpOptions.StatusCodeCheck.HasValue && (int)response.StatusCode != _httpOptions.StatusCodeCheck.Value)
            {
                CreateException(HttpLivenessErrors.INVALID_STATUS_CODE);
            }

            var content = await response.Content.ReadAsStringAsync();

            if(!string.IsNullOrEmpty(_httpOptions.ContentCheck) && !content.Contains(_httpOptions.ContentCheck))
            {
                CreateException(HttpLivenessErrors.INVALID_RESPONSE_CONTENT);
            }                       
        }

        private void CreateException(string message)
        {
            throw new Exception($"Error checking {_httpOptions.Url}: {message}");
        }
    }
}
