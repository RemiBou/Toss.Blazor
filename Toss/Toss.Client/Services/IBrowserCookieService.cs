using System;

namespace Toss.Client.Services
{
    public interface IBrowserCookieService
    {
        string Get(Func<string, bool> filterCookie);
    }
}