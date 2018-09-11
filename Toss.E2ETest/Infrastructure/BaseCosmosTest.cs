using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toss.Server.Data;
using Xunit;

namespace Toss.Tests.Infrastructure
{



    /// <summary>
    /// This test classe will drop and recreate a test cosmos database at each test
    /// </summary>
    public class BaseCosmosTest : IAsyncLifetime
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


        protected ICosmosDBTemplate<T> GetTemplate<T>()
        {
            return new CosmosDBTemplate<T>(_client, new CosmosDBTemplateOptions() { DataBaseName = _databaseName });
        }

        

        public async Task InitializeAsync()
        {
        }

        public async Task DisposeAsync()
        {
            var collections = _client.CreateDocumentCollectionQuery(UriFactory.CreateDatabaseUri(_databaseName)).ToList();
            foreach (var item in collections)
            {
                var docs =  _client.CreateDocumentQuery(item.SelfLink);
                foreach (var d in docs)
                {
                    await _client.DeleteDocumentAsync(d.SelfLink);
                }
            }
            await Task.Delay(100);//mandatory for issuing queries after collection deletion
        }
    }
}
