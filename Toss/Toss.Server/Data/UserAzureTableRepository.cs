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
        /// Return the user name for each user key send,
        /// If a user key doesn't exists, it's not in the output
        /// </summary>
        /// <param name="rowKeys"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetUserNames(IEnumerable<string> rowKeys)
        {
            var res = new ConcurrentDictionary<string, string>();
            var tasks = rowKeys.Select(k => appDbContext.UserTable
             .ExecuteAsync(TableOperation.Retrieve(k, k, new List<string> { "UserName" }))
             .ContinueWith((t) =>
             {
                 var tableEntity = (DynamicTableEntity)t.Result.Result;
                 res.TryAdd(k, tableEntity.Properties["UserName"].StringValue);
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
