using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toss.Server.Data
{
    public class UserAzureTableRepository : IUserRepository
    {
        /// <summary>
        /// Return the user name for each user key send,
        /// If a user key doesn't exists, it's not in the output
        /// </summary>
        /// <param name="rowKeys"></param>
        /// <returns></returns>
        public async Task<Dictionary<string,string>> GetUserNames(IEnumerable<string> rowKeys)
        {
            throw new NotImplementedException();
        }
    }
}
