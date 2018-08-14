using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toss.Server.Data;

namespace Toss.Tests.Infrastructure
{



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
            _databaseName = CosmosDBFixture.DatabaseName;
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
            return new CosmosDBTemplate<T>(_client, new CosmosDBTemplateOptions() { DataBaseName = _databaseName });
        }
    }
}
