using Microsoft.AspNetCore.Blazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toss.Shared;
using Toss.Shared.Account;

namespace Toss.Client.Services
{
    /// <summary>
    /// Access to account informations
    /// </summary>
    public interface IAccountService
    {

        /// <summary>
        /// Get the current user account details from backend
        /// </summary>
        /// <returns></returns>
        Task<AccountViewModel> CurrentAccount();

        Task Logout();

        event EventHandler OnLogoutDone;
        event EventHandler OnLoginDone;

        Task<Dictionary<string, List<string>>> Login(LoginCommand loginCommand, ElementRef loginButton);

    }
}