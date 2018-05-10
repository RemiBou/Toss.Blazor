using System.Threading.Tasks;
using Toss.Shared;

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
    }
}