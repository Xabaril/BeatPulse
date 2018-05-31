
# Prometheus Tracker

## About Prometheus

[Prometheus](http://www.prometheus.io) is an open-source systems monitoring and alerting toolkit originally built at SoundCloud. Since its inception in 2012, many companies and organizations have adopted Prometheus, and the project has a very active developer and user community. It is now a standalone open source project and maintained independently of any company. To emphasize this, and to clarify the project's governance structure, Prometheus joined the Cloud Native Computing Foundation in 2016 as the second hosted project, after Kubernetes.

## Test Prometheus tracker

In order to test our prometheus tracker on **samples\Prometheus-Stack-Environment** you have a Docker compose file to create a simple Prometheus stack with:

1. Prometheus Database
2. Prometheus Gateway
3. Grafana Dashboards

In order to test the Prometheus tracker you can use the Trackers sample and ensure the tracker for Prometheus is not commented.

```csharp
    setup.AddPrometheusTracker(new Uri("http://localhost:9091"), new Dictionary<string, string>()
    {
        {"MachineName",Environment.MachineName}
    });
```

Now can use Grafana to create dashboards using the *BeatPulse* information.

![Grafana Prometheus Query](./prometheusgrafanaquery.PNG)

## Installation

``` Powershell
Install-Package BeatPulse.PrometheusTracker
```

## Configuration

```csharp
    public class Startup
    {       
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBeatPulse(setup =>
            {
                setup.AddPrometheusTracker(new Uri("http://prometheus-gateway-uri"));
            });
        }
    }
```
