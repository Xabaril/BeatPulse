
# StatusPage Tracker

## About StatusPage

[StatusPage](https://www.statuspage.io) is the #1 status and incident communication tool.

## Installation

``` Powershell
Install-Package BeatPulse.StatusPageTracker
```

## Configuration

```csharp
    public class Startup
    {       
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBeatPulse(setup =>
            {
                setup.AddStatusPageTracker(opt =>
                {
                   opt.PageId = "your-page-id";
                   opt.ComponentId = "your-component-id";
                   opt.ApiKey = "your-api.key";
                   opt.IncidentName = "BeatPulse mark this component as outage";
                });
            });
        }
    }
```
