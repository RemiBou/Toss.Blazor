using Xunit;

namespace Toss.Tests.Infrastructure
{
    [CollectionDefinition("AzureTablecollection")]
    public class AzureTableFixtureCollection : ICollectionFixture<AzureTableFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}