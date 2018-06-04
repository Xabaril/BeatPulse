using BeatPulse.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BeatPulse.PrometheusTracker
{
    static class LivenessResultExtensions
    {
        public static string GetPrometheusMetrics(this LivenessResult response, IDictionary<string, string> prometheusLabels = null)
        {
            var builder = new StringBuilder();

            var labels = GetLabels(response, prometheusLabels);

            builder.Append($"beatpulse_pulse_execution_time_seconds{labels} { ((double)response.MilliSeconds / 1000)}\n");
            builder.Append($"beatpulse_pulse_ishealthy{labels} {Convert.ToInt32(response.IsHealthy)}\n");

            return builder.ToString();
        }

        static string GetLabels(LivenessResult response, IDictionary<string, string> prometheusLabels = null)
        {
            var labelsBuilder = new StringBuilder();

            var ApplicationName = @Assembly.GetEntryAssembly().GetName().Name;

            if (prometheusLabels != null)
            {
                if (prometheusLabels.ContainsKey(nameof(ApplicationName)))
                {
                    ApplicationName = prometheusLabels[nameof(ApplicationName)];
                }

                foreach (var entry in prometheusLabels)
                {
                    labelsBuilder.Append($"{entry.Key}=\"{entry.Value}\",");
                }
            }

            labelsBuilder.Append($"{nameof(ApplicationName)}=\"{ApplicationName}\",");
            labelsBuilder.Append($"{nameof(response.Path)}=\"{response.Path}\",");
            labelsBuilder.Append($"{nameof(response.Name)}=\"{response.Name}\",");


            labelsBuilder.Length -= 1; // remove latest ,

            return $"{{{labelsBuilder.ToString()}}}";
        }
    }
}
