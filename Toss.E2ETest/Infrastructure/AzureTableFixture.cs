using Microsoft.AspNetCore.Identity.DocumentDB;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toss.Server.Data;
using Toss.Server.Models;

namespace Toss.Tests.Infrastructure
{
    public class AzureTableFixture : IDisposable
    {
        public string TablePrefix { get; set; } = "UnitTests";
        public UserStore<ApplicationUser> UserStore;

        public DocumentClient Client;
        public Database Database;

        public AzureTableFixture()
        {

            Database = Client.CreateDatabaseQuery()
                                .Where(d => d.Id == "UnitTest")
                                .AsEnumerable()
                                .FirstOrDefault();
            if (Database != null)
                Client.DeleteDatabaseAsync(Database.SelfLink);
            Database = Client.CreateDatabaseAsync(new Database { Id = "UnitTest" }).Result;

            UserStore = new UserStore<ApplicationUser>(Client, new DocumentCollection() { Id = "users");
        }
        public void Dispose()
        {
            Client.DeleteDatabaseAsync(Database.SelfLink);
        }
    }
}
