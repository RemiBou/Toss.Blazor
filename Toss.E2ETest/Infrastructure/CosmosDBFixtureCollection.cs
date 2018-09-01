using Xunit;

namespace Toss.Tests.Infrastructure
{
    [CollectionDefinition("CosmosDBFixture Collection")]
    public class CosmosDBFixtureCollection : ICollectionFixture<CosmosDBFixture>
    {
    }
}
 