using System.Collections.Generic;
using System.Linq;
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
        private readonly CloudTableClient storageClient;
        private readonly IUserRepository userRepository;
        private CloudTable mainTable;

        public TossAzureTableRepository(
            CloudTableClient storageClient,
            IUserRepository userRepository,
            string tablePrefix = null)
        {
            this.storageClient = storageClient;
            this.userRepository = userRepository;
            mainTable = storageClient.GetTableReference(tablePrefix+"Toss");
            
        }

        /// <summary>
        /// Return the X last toss created.
        /// If none was created an empty list is returned
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TossLastQueryItem>> Last(int count)
        {
            await mainTable.CreateIfNotExistsAsync();
            var result = new List<TossLastQueryItem>();
            // Create the table query.
            var query = new TableQuery<OneTossEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "AllToss"))
                .Select(new List<string> { "Content", "UserId", "CreatedOn" });
            
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
                    CreatedOn = t.CreatedOn,
                    UserName = t.UserId
                })
                .Take(count - result.Count));
                //with ExecuteQuerySegmentedAsync we must keep track of the number of item currently taken from DB
                //cf https://github.com/Azure/azure-storage-net/issues/323
                if (result.Count == count)
                    break;
            } while (tableContinuationToken != null);
            var userNameMapping = await userRepository.GetUserNames(result.Select(r => r.UserName).Distinct().ToList());
            foreach (var item in result)
            {
                item.UserName = userNameMapping[item.UserName];
            }
            return result;


        }

        /// <summary>
        /// Saves a toss on the DB, the command should be validated before
        /// </summary>
        /// <param name="createCommand"></param>
        /// <returns></returns>
        public async Task Create(TossCreateCommand createCommand)
        {
            await mainTable.CreateIfNotExistsAsync();

            var toss = new OneTossEntity(createCommand.Content,createCommand.UserId, createCommand.CreatedOn);

            await mainTable.ExecuteAsync(TableOperation.Insert(toss));
        }
    }
}
