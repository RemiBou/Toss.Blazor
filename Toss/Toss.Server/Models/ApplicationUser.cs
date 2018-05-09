using ElCamino.AspNetCore.Identity.AzureTable.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Toss.Server.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUserV2
    {
        public HashSet<string> Hashtags { get; set; }

        public string HashtagsJson
        {
            get
            {
                return JsonConvert.SerializeObject(Hashtags);
            }
            set
            {
                Hashtags = JsonConvert.DeserializeObject<HashSet<string>>(value);
            }
        }
    }
}
