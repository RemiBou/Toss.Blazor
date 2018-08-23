
using Microsoft.AspNetCore.Identity.DocumentDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Toss.Server.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
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
        /// <summary>
        /// Add the tag to the list of hashtags
        /// </summary>
        /// <param name="newTag"></param>
        public void AddHashTag(string newTag)
        {
            if (Hashtags == null)
                Hashtags = new HashSet<string>();
            Hashtags.Add(newTag);
        }

        internal bool AlreadyHasHashTag(string newTag)
        {
            return Hashtags != null && Hashtags.Contains(newTag);
        }

        internal void RemoveHashTag(string hashTag)
        {
            if (Hashtags == null)
                return;
            Hashtags.Remove(hashTag);
        }
    }
}
