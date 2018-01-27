using Microsoft.AspNetCore.TestHost;

namespace UnitTests.Base
{
    public class DefaultServerFixture
    {
        public TestServer Server { get; private set; }

        public DefaultServerFixture()
        {
            Server =  new TestServerBuilder()
                .WithPulsePath("hc")
                .Build();
        }
    }
}
