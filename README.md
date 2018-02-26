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
                .UseBeatPulse() //this add beat pulse startup filter
                .UseStartup<Startup>()
                .Build();
```

4. Configure *BeatPulse* middleware and the liveness libraries to be used.

``` csharp

    services.AddBeatPulse(setup =>
    {
        setup.AddSqlServer("your-sql-server-connection-string");
    });
```

5. Use the *BeatPulse* uri's to get liveness results.

```
http://your-project-uri/hc get the global information of all liveness checkers included in the configuration.

http://your-project-uri/_self get the liveness status of the project without execute any configured liveness library. This is ideal for your liveness uri setup in k8s pod configuration.

http://your-project-uri/[liveness-segment-path] get the liveness status of the specified liveness libraries. Each liveness library define a specified path. By default the Sql Server livenes library is *sqlserver*, for Redis is *redis*, for Postgress SQL is *npgsql* and for MongoDb is *mongodb*.
```