using Xunit;

namespace Toss.Tests.Infrastructure
{
    [CollectionDefinition("CosmosDBFixture")]
    public class CosmosDBFixtureCollection : ICollectionFixture<CosmosDBFixture>
    {
    }
}
