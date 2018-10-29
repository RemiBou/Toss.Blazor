using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toss.Shared
{
    public class AccountViewModel
    {
        public bool IsEmailConfirmed { get; set; }
       
        public string Email { get; set; }

        public List<string> Hashtags { get; set; } = new List<string>();

        /// <summary>
        /// If true the user uses a password for connection, if false he uses a OpenID provider
        /// </summary>
        public bool HasPassword { get; set; }
        public bool IsAdmin { get; set; }
    }
}
