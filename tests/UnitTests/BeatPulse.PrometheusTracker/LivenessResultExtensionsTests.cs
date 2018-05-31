using BeatPulse.Core;
using BeatPulse.PrometheusTracker;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace UnitTests.BeatPulse.PrometheusTracker
{
    public class livenessresult_extensions_should
    {
        [Fact]
        public void get_valid_prometheus_metrics_content()
        {
            string path = "sql";
            string name = "liveness1";

            var livenessResult = new LivenessResult(name, path);
            livenessResult.StartCounter();
            livenessResult.StopCounter("ok", true);


            livenessResult.GetPrometheusMetrics()
                .Should()
                .Contain($"beatpulse_pulse_execution_time_seconds{{ApplicationName=\"xunit.console\",Path=\"{path}\",Name=\"{name}\"}}");

            livenessResult.GetPrometheusMetrics()
                .Should()
                .Contain($"beatpulse_pulse_ishealty{{ApplicationName=\"xunit.console\",Path=\"{path}\",Name=\"{name}\"}}");
        }

        [Fact]
        public void get_valid_prometheus_metrics_content_with_custom_labels()
        {
            string path = "sql";
            string name = "liveness1";

            var customLabels = new Dictionary<string, string>()
            {
                {"labelA","valueA" },
                {"labelB","valueB" },
            };

            var livenessResult = new LivenessResult(name, path);
            livenessResult.StartCounter();
            livenessResult.StopCounter("ok", true);

            var prometheusMetrics = livenessResult.GetPrometheusMetrics(customLabels);

            prometheusMetrics
                .Should()
                .Contain($"beatpulse_pulse_execution_time_seconds{{labelA=\"valueA\",labelB=\"valueB\",ApplicationName=\"xunit.console\",Path=\"{path}\",Name=\"{name}\"}}");

            prometheusMetrics
                .Should()
                .Contain($"beatpulse_pulse_ishealty{{labelA=\"valueA\",labelB=\"valueB\",ApplicationName=\"xunit.console\",Path=\"{path}\",Name=\"{name}\"}}");
        }
    }
}
