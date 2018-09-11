using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BeatPulse.UI.Discovery.Kubernetes
{
    internal class KubernetesClient : IDisposable
    {
        private readonly Uri _host;
        private readonly string _token;
        private HttpClient _httpClient;

        public KubernetesClient(string host, string token)
        {
            if (!Uri.TryCreate(host, UriKind.Absolute, out _host))
            {
                throw new Exception($"{nameof(host)} is not a valid Uri");
            }
            _token = token;

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = _host;

            if (!string.IsNullOrEmpty(_token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            }
        }

        public async Task<ServiceList> GetServices(string label = "")
        {
            var response = await _httpClient.GetAsync($"{_host.AbsoluteUri}{KubernetesApiEndpoints.ServicesV1}?labelSelector={label}");
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ServiceList>(content);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
