using System.Collections.Generic;

namespace Toss.Server.Data.Indexes
{
    public class TossConnectedTagsIndex
    {
        public string Tag { get; internal set; }
        public IEnumerable<string> ConnectedTags { get; internal set; }
    }
}