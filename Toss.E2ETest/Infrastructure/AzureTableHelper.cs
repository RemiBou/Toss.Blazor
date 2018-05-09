using ElCamino.AspNetCore.Identity.AzureTable.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Toss.Server.Data;

namespace Toss.Tests.Infrastructure
{
    class AzureTableHelper
    {
        public static ApplicationDbContext GetApplicationDbContext()
        {
           return new ApplicationDbContext(
                  new IdentityConfiguration()
                  {
                      StorageConnectionString = "UseDevelopmentStorage=true;",
                      TablePrefix = "UnitTests"
                  });
        }
    }
}
