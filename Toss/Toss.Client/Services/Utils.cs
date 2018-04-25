using Microsoft.AspNetCore.Blazor.Browser.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toss.Client.Services
{
    public static class Utils
    {
        public static void ConsoleLog(string message)
        {
            RegisteredFunction.Invoke<bool>("log", message);
        }
    }
}
