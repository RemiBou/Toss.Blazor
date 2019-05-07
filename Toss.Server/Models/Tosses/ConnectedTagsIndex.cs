using System.Collections.Generic;

namespace Toss.Server.Data.Indexes
{
    public class TossConnectedTagsIndex
    {
        public string Tag1 { get; internal set; }

        public string Tag2 { get; set; }

        public int Count { get; set; }
    }

}