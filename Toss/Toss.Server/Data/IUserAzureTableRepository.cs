using System.Collections.Generic;
using System.Threading.Tasks;

namespace Toss.Server.Data
{
    public interface IUserRepository
    {
        Task<Dictionary<string, string>> GetUserNames(IEnumerable<string> userIds);
        

    }
}