using System.Collections.Generic;
using System.Linq;

namespace Toss.Shared.Tosses
{
    public class ConnectedTags
    {

        public ConnectedTags()
        {
        }

        public ConnectedTags(IEnumerable<string> connectedTags)
        {
            this.Hashtags = connectedTags.ToArray();
        }

        public string[] Hashtags { get; set; }
    }
}