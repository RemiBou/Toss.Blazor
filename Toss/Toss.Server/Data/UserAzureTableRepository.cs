using ElCamino.AspNetCore.Identity.AzureTable.Helpers;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toss.Server.Data
{
    public class UserAzureTableRepository : IUserRepository
    {
        private ApplicationDbContext appDbContext;

        public UserAzureTableRepository(ApplicationDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        /// <summary>
        /// Return the user name for each user id send,
        /// If a user key doesn't exists, it's not in the output
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetUserNames(IEnumerable<string> userIds)
        {
            var res = new ConcurrentDictionary<string, string>();
            var tasks = userIds
                .Select(k => new { userId = k, key = KeyHelper.GenerateRowKeyUserName(k) })
                .Select(k => appDbContext.UserTable
             .ExecuteAsync(TableOperation.Retrieve(k.key, k.key, new List<string> { "UserName" }))
             .ContinueWith((t) =>
             {
                 var tableEntity = (DynamicTableEntity)t.Result.Result;
                 if (tableEntity != null)
                     res.TryAdd(k.userId, tableEntity.Properties["UserName"].StringValue);
             }))
             .ToList();
            foreach (var task in tasks)
            {
                await task;
            }
            return res.ToDictionary(r => r.Key, r => r.Value);
        }
    }
}
