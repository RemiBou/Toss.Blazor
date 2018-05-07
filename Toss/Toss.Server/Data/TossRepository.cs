using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toss.Shared;

namespace Toss.Server.Data
{
    public class TossAzureTableRepository : ITossRepository
    {
        private string azureStorageConnectionString;

        public TossAzureTableRepository(string azureStorageConnectionString)
        {
            this.azureStorageConnectionString = azureStorageConnectionString;
        }

        public async Task<IEnumerable<TossLastQueryItem>> Last(int count)
        {
            throw new NotImplementedException();
        }
        public async Task Create(TossCreateCommand oneToss)
        {
            throw new NotImplementedException();
        }
    }
}
