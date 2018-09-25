using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.StatusPageTracker
{
    class StatusPageClient
    {

        private readonly HttpClient _httpClient;
        private readonly StatusPageComponent _statusPageComponent;

        public StatusPageClient(StatusPageComponent statusPageComponent)
        {
            _statusPageComponent = statusPageComponent ?? throw new ArgumentNullException(nameof(statusPageComponent));

            _httpClient = new HttpClient(
                new StatusPageAuthorizationHandler(statusPageComponent.ApiKey))
            {
                BaseAddress = new Uri("https://api.statuspage.io/v1/")
            };
        }

        public async Task CreateIncident(string failure)
        {
            var incidentId = await FindUnresolvedComponentIncident();

            if (String.IsNullOrWhiteSpace(incidentId))
            {
                var incident = $"incident[name]={_statusPageComponent.IncidentName}&incident[status]=investigating&incident[body]={failure}&incident[component_ids][]={_statusPageComponent.ComponentId}&incident[components][{_statusPageComponent.ComponentId}]=major_outage";

                var content = new StringContent(incident, Encoding.UTF8, "application/x-www-form-urlencoded");

                await _httpClient.PostAsync($"pages/{_statusPageComponent.PageId}/incidents.json", content);
            }
        }

        public async Task SolveIncident()
        {
            var incidentId = await FindUnresolvedComponentIncident();

            if (!String.IsNullOrEmpty(incidentId))
            {
                var content = $"incident[status]=resolved&incident[components][{_statusPageComponent.ComponentId}]=operational";

                await _httpClient.PatchAsync($"pages/{_statusPageComponent.PageId}/incidents/{incidentId}.json", new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded"));
            }
        }

        async Task<string> FindUnresolvedComponentIncident()
        {
            var response = await _httpClient.GetAsync($"pages/{_statusPageComponent.PageId}/incidents/unresolved.json");

            var unresolved = JsonConvert.DeserializeObject<IEnumerable<WebStatusIncident>>(
                await response.Content.ReadAsStringAsync());

            if (unresolved.Any())
            {
                return unresolved.Where(ws => ws.Name.Equals(_statusPageComponent.IncidentName, StringComparison.InvariantCultureIgnoreCase))
                    .Select(ws => ws.Id)
                    .SingleOrDefault();
            }

            return null;
        }


        private class StatusPageAuthorizationHandler : DelegatingHandler
        {
            private readonly string _apiKey;

            public StatusPageAuthorizationHandler(string apiKey)
            {
                _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
                InnerHandler = new HttpClientHandler();
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers
                    .TryAddWithoutValidation("Authorization", $"OAuth {_apiKey}");

                return await base.SendAsync(request, cancellationToken);
            }
        }

        private class WebStatusIncident
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public string Status { get; set; }
        }
    }
}
