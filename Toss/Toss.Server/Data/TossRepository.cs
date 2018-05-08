using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Toss.Shared;

namespace Toss.Server.Data
{
    /// <summary>
    /// Azure table design 
    /// Toss will be queried by
    /// - last XXX by date of creation : 
    ///     partition key : "Toss"
    ///     row key : MaxTicks - Ticks at creation (mind the duplicates)
    ///     other cols : content, username, date of creation
    /// - eventual filter on tag : {pk : tag 3 first letter, rk : tag
    /// 
    /// </summary>
    public class TossAzureTableRepository : ITossRepository
    {
        private CloudTableClient storageClient;
        private CloudTable mainTable;

        public TossAzureTableRepository(CloudTableClient storageClient)
        {
            this.storageClient = storageClient;
            mainTable = storageClient.GetTableReference("Toss");
        }

        public async Task<IEnumerable<TossLastQueryItem>> Last(int count)
        {
            var result = new List<TossLastQueryItem>();
            // Create the table query.
            var query = new TableQuery<OneTossEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "AllToss"))
                ;
            query.TakeCount = count;
            TableContinuationToken tableContinuationToken = null;
            do
            {
                var queryResponse = await mainTable
                    .ExecuteQuerySegmentedAsync(query, tableContinuationToken);
                tableContinuationToken = queryResponse.ContinuationToken;
                result.AddRange(queryResponse.Results.Select(t => new TossLastQueryItem()
                {
                    Content = t.Content,
                    DateOfPost = t.Timestamp.DateTime,
                    UserName = t.UserRowKey
                })
                .Take(count - result.Count));
                if (result.Count == count)
                    break;
            } while (tableContinuationToken != null);
            return result;


        }
        public async Task Create(TossCreateCommand oneToss)
        {
            throw new NotImplementedException();
        }
    }
}
