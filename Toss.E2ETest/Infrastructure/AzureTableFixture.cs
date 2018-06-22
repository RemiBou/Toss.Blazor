using ElCamino.AspNetCore.Identity.AzureTable;
using ElCamino.AspNetCore.Identity.AzureTable.Model;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using Toss.Server.Data;
using Toss.Server.Models;

namespace Toss.Tests.Infrastructure
{
    public class AzureTableFixture : IDisposable
    {
        public string TablePrefix { get; set; } = "UnitTests";
        public UserStoreV2<ApplicationUser, IdentityRole, ApplicationDbContext> UserStoreV2 ;

        public ApplicationDbContext applicationDbContext = AzureTableHelper.GetApplicationDbContext();
        public CloudTableClient storageClient;
        public CloudTable TossTable;

        public AzureTableFixture()
        {
            UserStoreV2 = new UserStoreV2<ApplicationUser, IdentityRole, ApplicationDbContext>(applicationDbContext);
            UserStoreV2.CreateTablesIfNotExists().Wait();

            var storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true;");
            storageClient = storageAccount.CreateCloudTableClient();

            TossTable = storageClient.GetTableReference(TablePrefix+"Toss");
            TossTable.CreateIfNotExistsAsync().Wait();
        }
        public void Dispose()
        {
            applicationDbContext.IndexTable.DeleteAsync().Wait();
            applicationDbContext.RoleTable.DeleteAsync().Wait();
            TossTable.DeleteIfExistsAsync().Wait();
            applicationDbContext.UserTable.DeleteAsync().Wait();
        }
    }
}
