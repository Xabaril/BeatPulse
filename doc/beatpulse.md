# Beat Pulse Requests

By default, the global path returns the aggregated state of all liveness checkers, including the out of box *self* check added. 

If all liveness are up and running, *BeatPulse* returns HTTP 200 OK.

``` bash
curl http://your-domain/hc 
GET /hc HTTP/1.1
Host: your-domain
User-Agent: curl/7.49.0
Accept: */*
HTTP/1.1 200 OK
OK
```

If some liveness is not working, *BeatPulse* returns HTTP 503 ServiceUnavailable.

``` bash
curl http://your-domain/hc 
GET /hc HTTP/1.1
Host: your-domain
User-Agent: curl/7.49.0
Accept: */*
HTTP/1.1 503 ServiceUnavailable
ServiceUnavailable
```

When *DetailedOutput* is true the information is a complete json result with liveness, time, and execution results. If you use *BeatPulse UI* detailed information is mandatory.

``` bash

curl http://your-domain/hc
GET /hc HTTP/1.1
Host: your-domain
User-Agent: curl/7.49.0
Accept: */*
HTTP/1.1 200 OK
{
    "checks": [
    {
        "name": "self",
        "path":"_self",
        "message": "OK",
        "milliSeconds": 0,
        "run": true,
        "isHealthy": true
    },
    {
        "name": "cat",
        "path":"catapi",
        "message": "OK",
        "milliSeconds": 376,
        "run": true,
        "isHealthy": true
    },
    {
        "name": "SqlServerLiveness",
        "path":"sqlserver",
        "message": "OK",
        "milliSeconds": 309,
        "run": true,
        "isHealthy": true
    }],
    "startedAtUtc": "2018-02-26T19:30:05.4058955Z",
    "endAtUtc": "2018-02-26T19:30:06.0978236Z",
    "code": "200",
    "reason":""
}
```

If you need to know the status of a particular service you can add the liveess name as a new segment into the liveness uri. In our later case, if you need a liveness uri for SqlServer  add /sqlserver to the *BeatPulse* request uri. The flag *DetailedOutput* is also working with particular checks.

``` bash

curl http://your-domain/hc/sqlserver
GET /hc HTTP/1.1
Host: your-domain
User-Agent: curl/7.49.0
Accept: */*
HTTP/1.1 200 OK
{
    "checks": [
    {
        "name": "SqlServerLiveness",
        "path":"sqlserver",
        "message": "OK",
        "milliSeconds": 309,
        "run": true,
        "isHealthy": true
    }],
    "startedAtUtc": "2018-02-26T19:30:05.4058955Z",
    "endAtUtc": "2018-02-26T19:30:06.0978236Z",
    "code": "200",
    "reason":""
}
```
 
Out-of-box *BeatPulse* add a **Self** liveness in order to check only the web app and not the configured liveness. This is usefull on K8S to set the pod liveness for web app. The path for this liveness is **_self**.

# Cache responses

BeatPulse can cache its responses. There are two cache modes:

1. By using HTTP Headers. Using this model beatpulse adds a `Cache-Control` header with a value of  `public, max-age=xxx`. It is up to user agents to honor this header.
2. In-memory. Using this model beatpulse stores the previous response and returns if the cache duration time has not elapsed.

To enable cache use the method `EnableOutputCache`:

``` csharp
    .UseBeatPulse(options=>
    {
        options.ConfigurePath(path:"health") //default hc
            .ConfigureOutputCache(seconds:10)      // Can use CacheMode as second parameter
            .ConfigureTimeout(milliseconds:1500) // default -1 infinitely
            .ConfigureDetailedOutput(detailedOutput:true); //default false
    })
```

You can specify the cache method by using a second parameter with a `CacheMode` value (`Header`, `ServerMemory` or `HeaderAndServerMemory`) (default is `Header`).

If you perform two inmediate requests (because an user-agent that does not follow the `Cache-Control` header is being used) and in-memory cache is enabled you will receive the same response both times and all checks will be performed only once. If in-memory cache is not enabled all checks will be performed again.

# Authentication

*BeatPulse* supports a simple authentication mechanism in order to pre-authenticate *BeatPulse* requests using the **IBeatPulseAuthenticationFilter**.

```csharp
    public interface IBeatPulseAuthenticationFilter
    {
        Task<bool> IsValid(HttpContext httpContext);
    }
```

Out-of-box you can authenticate requests using an **api-key** query string parameter by configuring the filter below in your service collection.

```csharp
    services.AddSingleton<IBeatPulseAuthenticationFilter>(new ApiKeyAuthenticationFilter("api-key-secret"));
```

With this filter, only requests with the **api-key** query string parameter can get results, http://your-server/health?api-key=api-key-secret.

You can create your custom **IBeatPulseAuthenticationFilter** filters by creating a new implementation and registering it in the service collection.

Below you have a custom authentication filter that verifies a header to authenticate the request:

```csharp
    public class HeaderValueAuthenticationFilter : IBeatPulseAuthenticationFilter
    {
        private readonly string _headerName;
        private readonly string _headerValue;

        public HeaderValueAuthenticationFilter(string headerName, string headerValue)
        {
            _headerName = headerName ?? throw new ArgumentNullException(nameof(headerName));
            _headerValue = headerValue ?? throw new ArgumentNullException(nameof(headerValue));
        }
        public Task<bool> IsValid(HttpContext httpContext)
        {
            var header = httpContext.Request.Headers[_headerName];

            if (String.Equals(header, _headerValue, StringComparison.InvariantCultureIgnoreCase))
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
```

```csharp
    services.AddSingleton<IBeatPulseAuthenticationFilter>(
        new HeaderValueAuthenticationFilter("header1", "value1"));
```

## Enabling CORS (and other existing middlewares) on BeatPulse pipeline.

If you need to include other existing pipelines, like CORS, on **BeatPulse** you can configure the *BeatPulseMiddleware* using a *IApplicationBuilder* extension method instead a *StartupFilter*.

Next sample show a *Startup.cs* with **CORS** support.

```csharp
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddCors();
            services.AddBeatPulse(setup =>
            {
                setup.AddLiveness("sample", opt =>
                 {
                     opt.UsePath("path");
                     opt.UseLiveness(new ActionLiveness((_, __) => Task.FromResult(("Ok", true))));
                 });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(setup =>
            {
                setup.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowCredentials();
            });

            app.UseBeatPulse(setup =>
            {
                setup.ConfigureDetailedOutput(true)
                    .ConfigurePath("hc");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
```

You can also add specific middlewares only for *BeatPulse*. Next sample show howto add *CORS* support only for our *BeatPulse* requests.

```csharp
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddCors();
            services.AddBeatPulse(setup =>
            {
                setup.AddLiveness("sample", opt =>
                 {
                     opt.UsePath("path");
                     opt.UseLiveness(new ActionLiveness((_, __) => Task.FromResult(("Ok", true))));
                 });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseBeatPulse(setup => { }, builder =>
            {
                builder.UseCors(setup =>
                {
                    setup.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin()
                        .AllowCredentials();
                });
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
```