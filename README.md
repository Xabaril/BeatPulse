[![Build status](https://ci.appveyor.com/api/projects/status/fqcak0q6q83f730c?svg=true)](https://ci.appveyor.com/project/Xabaril/beatpulse) [![NuGet](https://img.shields.io/nuget/v/BeatPulse.svg)](https://www.nuget.org/packages/BeatPulse/)

[![Build history](https://buildstats.info/appveyor/chart/xabaril/beatpulse)](https://ci.appveyor.com/project/beatpulse/ci-buildstats/history)

# Beat Pulse

*BeatPulse* is a simple health check /  liveness / readiness library for .NET Core Applications.


## What is the motivation behind it?

The [Microsoft HealthCheck](https://github.com/dotnet-architecture/HealthChecks) library is not an active project right now and there is no plan to include this feature in ASP.NET Core 2.1.

## Getting Started

1. Install the Nuget Package into your ASP.NET Core application.

```
Install-Package BeatPulse
```

2. Install the liveness libraries that you need your project. At this moment *BeatPulse* contains libraries for *Redis, SqlServer, MongoDb, Postgress Sql*.

```
Install-Package BeatPulse.SqlServer
Install-Package BeatPulse.MongoDb
Install-Package BeatPulse.Npgsql
Install-Package BeatPulse.Redis
```

3. Add *BeatPulse* into your ASP.NET Core project

``` csharp

 public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
               .UseBeatPulse(options=>
                {
                   options.DetailedOutput = true; // default false
                   options.BeatPulsePath = "health"; // default hc
                }).UseStartup<Startup>().Build();
```
4. Configure *BeatPulse* middleware and the liveness libraries to be used or add a custom check.

``` csharp
    services.AddBeatPulse(setup =>
    {
        //add custom health check
        setup.Add(new ActionHealthCheck("cat", "catapi", async  httpContext =>
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://www.google.es")
            };

            var response = await httpClient.GetAsync(string.Empty);
                
            if (response.IsSuccessStatusCode)
            {
                return ("OK", true);
            }
            else
            {
                return ("the cat api is broken!", false);
            }
            
        }));

        //add sql server health check
        setup.AddSqlServer("your-connection-string");
    });
```

5. Use the *BeatPulse* uri's to get liveness/readiness results.

By default, the global path get the information of all liveness checkers, including the automatic *self* check added. If *DetailedOutput* is true the information is a complete json result with checkers, time, and results.

``` json

curl curl http://your-domain/health
GET /health HTTP/1.1
Host: your-domain
User-Agent: curl/7.49.0
Accept: */*
HTTP/1.1 200 OK

{
	"Checks": [
		{
			"Name": "self",
			"Message": "OK",
			"MilliSeconds": 0,
			"Run": true,
			"IsHealthy": true
		},
		{
			"Name": "cat",
			"Message": "OK",
			"MilliSeconds": 376,
			"Run": true,
			"IsHealthy": true
		},
		{
			"Name": "SqlServerHealthCheck",
			"Message": "OK",
			"MilliSeconds": 309,
			"Run": true,
			"IsHealthy": true
		}
	],
	"StartedAtUtc": "2018-02-26T19:30:05.4058955Z",
	"EndAtUtc": "2018-02-26T19:30:06.0978236Z"
}
```
Optionally, you can get result for specific liveness adding the liveness segment path on the beat pulse base path.

http://your-domain/health/_self get the liveness status of the project without execute any configured liveness library. This is usual for the liveness path on k8s pods.

http://your-domain/health/[liveness-segment-path] get the liveness status of the specified liveness libraries. Each liveness library define a specified path. By default the Sql Server livenes library is *sqlserver*, for Redis is *redis*, for Postgress SQL is *npgsql* and for MongoDb is *mongodb*.


