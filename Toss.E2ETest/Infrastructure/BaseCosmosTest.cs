using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toss.Server.Data;
using Xunit;

namespace Toss.Tests.Infrastructure
{
    [CollectionDefinition("CosmosDBFixture")]
    public class CosmosDBFixtureCollection : ICollectionFixture<CosmosDBFixture>
    {
    }
    public class CosmosDBFixture : IDisposable
    {
        public CosmosDBFixture()
        {
            var config = new ConfigurationBuilder()
               .AddEnvironmentVariables()
               .Build();
            Client = new DocumentClient(new Uri("https://localhost:8081"), "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");//same local key for everyone !
            Client.CreateDatabaseAsync(new Microsoft.Azure.Documents.Database() { Id = DatabaseName });
            // ... initialize data in the test database ...
        }

        public void Dispose()
        {

            Client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseName)).Wait();
        }

        public DocumentClient Client { get; private set; }
        public string DatabaseName { get; internal set; } = "UnitTests";
    }



    /// <summary>
    /// This test classe will drop and recreate a test cosmos database at each test
    /// </summary>
    public class BaseCosmosTest : IDisposable
    {
        private readonly CosmosDBFixture cosmosDBFixture;
        protected DocumentClient _client;
        private string _databaseName;

        public BaseCosmosTest(CosmosDBFixture cosmosDBFixture)
        {
            this.cosmosDBFixture = cosmosDBFixture;
            _client = cosmosDBFixture.Client;
            _databaseName = cosmosDBFixture.DatabaseName;
        }

        /// <summary>
        /// Delete all the collections after each tests so we don't have any document left
        /// </summary>
        public void Dispose()
        {
            var collections = _client.CreateDocumentCollectionQuery(UriFactory.CreateDatabaseUri(_databaseName)).ToList();
            foreach (var item in collections)
            {
                _client.DeleteDocumentCollectionAsync(item.SelfLink).Wait();
            }
        }

        protected ICosmosDBTemplate<T> GetTemplate<T>()
        {
            return new CosmosDBTemplate<T>(_client, _databaseName);
        }
    }
}
