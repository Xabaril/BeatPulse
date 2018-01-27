using Xunit;

namespace UnitTests.Base
{
    [CollectionDefinition(DefaultServerCollectionFixture.Default)]
    public class DefaultServerCollectionFixture
        :ICollectionFixture<DefaultServerFixture>
    {
        public const string Default = nameof(Default);
    }
}
