using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Toss.Tests.Infrastructure
{
    public class CosmosDBFixture : IDisposable
    {
        public CosmosDBFixture()
        {
            var config = new ConfigurationBuilder()
               .AddEnvironmentVariables()
               .Build();
            Client = new DocumentClient(new Uri("https://localhost:8081"), "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            });//same local key for everyone !
            
            
            Client.CreateDatabaseIfNotExistsAsync(new Microsoft.Azure.Documents.Database() { Id = DatabaseName }).Wait();
            // ... initialize data in the test database ...
        }

        public void Dispose()
        {
            /*var collections = Client.CreateDocumentCollectionQuery(UriFactory.CreateDatabaseUri(DatabaseName)).ToList();
            foreach (var item in collections)
            {
                Client.DeleteDocumentCollectionAsync(item.SelfLink).Wait();
            }*/

            Client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseName)).Wait();
        }

        public DocumentClient Client { get; private set; }
        public const string DatabaseName  = "UnitTests";
    }
}
