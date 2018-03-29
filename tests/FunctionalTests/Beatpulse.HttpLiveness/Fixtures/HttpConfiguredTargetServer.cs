using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalTests.Beatpulse.Http.Fixtures
{
    public class HttpConfiguredTargetServer: IDisposable
    {
        public const string DefaultTargetServerUrl = "http://localhost:54000";
        private readonly RequestDelegate _delegate;
        private IWebHost _server = null;

        public HttpConfiguredTargetServer(RequestDelegate @delegate, string url = DefaultTargetServerUrl)
        {
            _server = new WebHostBuilder()
                .UseUrls(new[] { url })
                .UseKestrel()
                .Configure(app =>
                {
                    app.Run(@delegate);
                })
            .Build();
            //Blocking operation on tests to achieve IDisposable server
            _server.StartAsync().GetAwaiter().GetResult();
        }       
        public void Dispose()
        {
            //Blocking operation on tests to achieve IDisposable server
            _server.StopAsync().GetAwaiter().GetResult();
        }
    }
}
