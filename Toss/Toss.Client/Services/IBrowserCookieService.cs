using System;
using System.Threading.Tasks;

namespace Toss.Client.Services
{
    public interface IBrowserCookieService
    {
        Task<string> Get(Func<string, bool> filterCookie);
    }
}