using BeatPulse.Core;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BeatPulse.PrometheusTracker
{
    class PrometheusGatewayTracker
        : IBeatPulseTracker
    {
        private static HttpClient _httpClient;

        public string Name => "Prometheus";

        private Uri _prometheusGatewayUri;
        private IDictionary<string, string> _prometheusLabels;

        public PrometheusGatewayTracker(Uri prometheusGatewayUri,IDictionary<string,string> customLabels = null)
        {
            _prometheusGatewayUri = prometheusGatewayUri ?? throw new ArgumentNullException(nameof(prometheusGatewayUri));
            _prometheusLabels = customLabels;

            _httpClient = new HttpClient()
            {
                BaseAddress = _prometheusGatewayUri
            };
        }

        public Task Track(LivenessResult response)
        {
            const string PROMETHEUS_METRIC_JOB_PATH = "metrics/jobs/beatpulse";

            //this tracker use prometheus gateway to send 
            //liveness results into prometheus with a push model
            //for more information about prometheus please read https://prometheus.io/

            var prometheusMetrics = response.GetPrometheusMetrics(_prometheusLabels);

            var content = new StringContent(prometheusMetrics, Encoding.UTF8, "multipart/form-data");

            return  _httpClient.PostAsync(PROMETHEUS_METRIC_JOB_PATH, content);
        }
    }
}
