using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toss.Server.Services
{
    /// <summary>
    /// This class is here for return the system current DateTimeOffset
    /// </summary>
    public class Now : INow
    {
        public DateTimeOffset Get()
        {
            return DateTimeOffset.Now;
        }
    }
}
