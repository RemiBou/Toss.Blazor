using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toss.Shared;

namespace Toss.Client.Services
{
    public class AccountService : IAccountService
    {
        private IHttpApiClientRequestBuilderFactory http;

        public AccountService(IHttpApiClientRequestBuilderFactory http)
        {
            this.http = http;
        }
        /// <summary>
        /// Get the current user account details from backend
        /// </summary>
        /// <returns></returns>
        public async Task<AccountViewModel> CurrentAccount()
        {
            AccountViewModel account = null;
            await http.Create("/api/account/details")
              .OnOK<AccountViewModel>((a) => account = a)
              .Get();
            return account;
        }
    }
}
