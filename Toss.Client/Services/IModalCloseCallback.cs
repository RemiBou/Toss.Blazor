using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor;

namespace Toss.Client.Services
{
    public interface IModalCloseCallback
    {
       Task OnClose();
    }
}
