using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toss.Server.Data;

namespace Toss.Tests.Infrastructure
{

    public class CosmosDBFixture : IDisposable
    {
        public CosmosDBFixture()
        {
            var config = new ConfigurationBuilder()
               .AddJsonFile("client-secrets.json")
               .Build();
            Client = new DocumentClient(new Uri(config["CosmosDBEndpoint"]), config["CosmosDBKey"]);
            Client.CreateDatabaseAsync(new Microsoft.Azure.Documents.Database() { Id = DatabaseName });
            // ... initialize data in the test database ...
        }

        public void Dispose()
        {
            var dbs = Client.CreateDatabaseQuery().ToList();
            foreach (var item in dbs)
            {
                Client.DeleteDatabaseAsync(item.SelfLink).Wait();
            }
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
