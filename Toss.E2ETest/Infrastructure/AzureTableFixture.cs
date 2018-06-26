using Microsoft.AspNetCore.Identity.DocumentDB;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toss.Server.Data;
using Toss.Server.Models;

namespace Toss.Tests.Infrastructure
{
    public class AzureTableFixture : IDisposable
    {
        public const string UnitTestsDatabaseId = "UnitTests";
        public UserStore<ApplicationUser> UserStore;

        public DocumentClient Client;
        public Database Database;

        public AzureTableFixture()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("client-secrets.json")
                .Build();
            Client = new DocumentClient(new Uri(config["CosmosDBEndpoint"]), config["CosmosDBKey"]);
          

            
        }


        public async Task  Init()
        {
            UserStore = new UserStore<ApplicationUser>(Client, new DocumentCollection() { Id = "users" });
            Database = Client.CreateDatabaseQuery()
                                .Where(d => d.Id == UnitTestsDatabaseId)
                                .AsEnumerable()
                                .FirstOrDefault();
            if (Database != null)
                await Client.DeleteDatabaseAsync(Database.SelfLink);
            Database = await Client.CreateDatabaseIfNotExistsAsync(new Database { Id = UnitTestsDatabaseId });
        }
        public async Task Clean()
        {
            await Client.DeleteDatabaseAsync(Database.SelfLink);
        }

        public void Dispose()
        {
        }
    }
}
