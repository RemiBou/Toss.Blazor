using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Toss.Shared;

namespace Toss.Server.Data
{
    /// <summary>
    /// Store and retreive Toss from an Azure Table Storage
    /// </summary>
    public class TossAzureTableRepository : ITossRepository
    {
        private const string PartitionKeyAllToss = "AllToss";
        private const string AllTossPartitionKeyCondition = "PartitionKey eq 'AllToss'";
        private readonly CloudTableClient storageClient;
        private CloudTable mainTable;

        public TossAzureTableRepository(
            CloudTableClient storageClient,
            string tablePrefix = null)
        {
            this.storageClient = storageClient;
            mainTable = storageClient.GetTableReference(tablePrefix + "Toss");

        }
        private static List<string> TossLastQueryItemColumns = new List<string> { "Content", "UserName", "CreatedOn" };
        /// <summary>
        /// Return the X last toss created.
        /// If none was created an empty list is returned
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TossLastQueryItem>> Last(int count, string hashTag)
        {
            await mainTable.CreateIfNotExistsAsync();
            if (hashTag == null)
            {
                
                var query = new TableQuery()
                    .Where(AllTossPartitionKeyCondition)
                    .Take(count)
                    .Select(TossLastQueryItemColumns);

               
                return (await mainTable
                    .ExecuteQueryAsync(query))
                    .Select(MapTossLastQueryItem)
                    .Take(count)//https://stackoverflow.com/a/13453691/277067
                    .ToList();
            }
            else
            {
                var query = new TableQuery()
                       .Where($"PartitionKey eq '{HashTagIndex.PartionKeyPrefix}{hashTag}'")
                       .Take(count)
                       .Select(new List<string> { "RowKey" });
                var queryResponse = await mainTable
                   .ExecuteQueryAsync(query);
                var rowKeys = (await mainTable
                    .ExecuteQueryAsync(query))
                    .Select(t => t.RowKey)
                    .Take(count)
                    .ToList();
                var tasks = rowKeys
                    .Select(r => mainTable.ExecuteAsync(
                       TableOperation.Retrieve(PartitionKeyAllToss, r, TossLastQueryItemColumns)
                    ))
                    .ToArray();
                await Task.WhenAll(tasks);
                return tasks
                    .Select(t => MapTossLastQueryItem((DynamicTableEntity)t.Result.Result))
                    .ToList();
            }
        }

        private static TossLastQueryItem MapTossLastQueryItem(DynamicTableEntity t)
        {
            return new TossLastQueryItem()
            {
                Content = t.Properties["Content"].StringValue,
                CreatedOn = t.Properties["CreatedOn"].DateTimeOffsetValue.Value,
                UserName = t.Properties["UserName"].StringValue,
            };
        }

        /// <summary>
        /// Saves a toss on the DB, the command should be validated before, 
        /// </summary>
        /// <param name="createCommand"></param>
        /// <returns></returns>
        public async Task Create(TossCreateCommand createCommand)
        {
            await mainTable.CreateIfNotExistsAsync();
            var tasks = new List<Task>();
            var toss = new OneTossEntity(createCommand.Content, createCommand.UserId, createCommand.CreatedOn);
           
            tasks.Add(mainTable.ExecuteAsync(TableOperation.Insert(toss)));
            tasks.AddRange(HashTagIndex.CreateHashTagIndexes(toss).Select(h => mainTable.ExecuteAsync(TableOperation.Insert(h))));

            await Task.WhenAll(tasks);
        }
    }
}
