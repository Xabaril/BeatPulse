[![Build status](https://ci.appveyor.com/api/projects/status/fqcak0q6q83f730c?svg=true)](https://ci.appveyor.com/project/Xabaril/beatpulse) [![NuGet](https://img.shields.io/nuget/v/BeatPulse.svg)](https://www.nuget.org/packages/BeatPulse/)

[![Build history](https://buildstats.info/appveyor/chart/xabaril/beatpulse)](https://ci.appveyor.com/project/xabaril/beatpulse/history)

# Beat Pulse

*BeatPulse* is a simple liveness, readiness library for .NET Core Applications.

## What is the motivation behind it

The [Microsoft HealthCheck](https://github.com/dotnet-architecture/HealthChecks) library is not an active project right now and there is no plan to include this feature in ASP.NET Core 2.1.

## Getting Started

1. Install the Nuget Package into your ASP.NET Core application.

``` PowerShell
Install-Package BeatPulse
```

2. Install the liveness libraries that you need on your project. At this moment *BeatPulse* contains libraries for *Redis, SqlServer, MongoDb, Postgress Sql, Azure Storage (Blobs, Tables and Queues), DocumentDb, MySQL, SqLite and custom lambda liveness*.

``` PowerShell
Install-Package BeatPulse.SqlServer
Install-Package BeatPulse.MongoDb
Install-Package BeatPulse.Npgsql
Install-Package BeatPulse.Redis
Install-Package BeatPulse.AzureStorage
Install-Package BeatPulse.MySql
Install-Package BeatPulse.DocumentDb
Install-Package BeatPulse.SqLite
```

3. Add *BeatPulse* into your ASP.NET Core project. *UseBeatPulse* is a new IWebHostBuilder extension method for register and configure BeatPulse.

``` csharp
 public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
               .UseBeatPulse(options=>
                {
                   options.SetAlternatePath("health") //default hc
                        .SetTimeout(milliseconds:1500) // default -1 infinitely
                        .EnableDetailedOutput(); //default false
                }).UseStartup<Startup>().Build();
```

4. Add *BeatPulseService* and set the liveness libraries to be used.

``` csharp
    services.AddBeatPulse(setup =>
    {
        //add custom liveness
        setup.Add(new ActionLiveness("cat", "catapi", async  httpContext =>
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

        //add sql server liveness
        setup.AddSqlServer("your-connection-string");
    });
```

5. Request *BeatPulse* to get liveness results.

By default, the global path get the information of all liveness checkers, including the out of box *self* check added. If *DetailedOutput* is true the information is a complete json result with liveness, time, and execution results.

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
        "Path":"_self",
        "Message": "OK",
        "MilliSeconds": 0,
        "Run": true,
        "IsHealthy": true
    },
    {
        "Name": "cat",
        "Path":"catapi",
        "Message": "OK",
        "MilliSeconds": 376,
        "Run": true,
        "IsHealthy": true
    },
    {
        "Name": "SqlServerHealthCheck",
        "Path":"sqlserver",
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

## Cache responses

BeatPulse can cache its responses. There are two cache modes:

1. By using HTTP Headers. Using this model beatpulse adds a `Cache-Control` header with a value of  `public, max-age=xxx`. It is up to user agents to honor this header.
2. In-memory. Using this model beatpulse stores the previous response and returns if the cache duration time has not elapsed.

To enable cache use the method `EnableOutputCache`:

``` csharp
    .UseBeatPulse(options=>
    {
        options.SetAlternatePath("health") //default hc
            .EnableOutputCache(10)      // Can use CacheMode as second parameter
            .SetTimeout(milliseconds:1500) // default -1 infinitely
            .EnableDetailedOutput(); //default false
    })
```

You can specify the cache method by using a second parameter with a `CacheMode` value (`Header`, `ServerMemory` or `HeaderAndServerMemory`) (default is `Header`).

If you perform two inmediate requests (because using a user-agent that do not follow the `Cache-Control` header) and in-memory cache is enabled you will receive the same response both times and all checks will be performed only once. If in-memory cache is not enabled all checks will be performed again.

## UI

The project BeatPulse.UI is a minimal UI interface for store and show the liveness results from the configured liveness uri's. To integrate BeatPulse.UI in your project you only need add the BeatPulse.UI services and middlewares.

```csharp
    public class Startup
    {       
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBeatPulseUI();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseBeatPulseUI();
        }
    }
```

This automatically register a new interface on **/beatpulse-ui**. Optionally, *UseBeatPulseUI* can be configured with different UI response path.

![BeatPulseUI](./doc/BeatPulseUI-1.PNG)

### Configuration

The liveness to be used on BeatPulse-UI are configured using the **BeatPulse-UI** settings.

```json
{
  "BeatPulse-UI": {
    "Liveness": [
      {
        "Name": "HTTP-Api-Basic",
        "Uri": "http://localhost:6457/health"
      }
    ],
    "WebHookNotificationUri": "",
    "EvaluationTimeOnSeconds": 10
  }
}
```

    1.- Liveness: The collection of the liveness uri to watch.
    2.- EvaluationTimeOnSeconds: Number of elapsed seconds between liveness checks.
    3.- WebHookNotificationUri: If any liveness return a *Down* result, this uri is used to notify the error status.

All the liveness result are stored into a SqLite database persisted to disk with *livenessdb* name.