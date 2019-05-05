using Raven.Client.Documents.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toss.Server.Data.Indexes
{
    public class Toss_ConnectedTags :  AbstractIndexCreationTask<TossEntity, TossConnectedTagsIndex>
    {
        public Toss_ConnectedTags()
        {
            Map = tosses => from toss in tosses
                            from tag in toss.Tags
                            select new TossConnectedTagsIndex()
                            {
                                Tag = tag,
                                ConnectedTags = toss.Tags.Where(t => t != tag)
                            };
            Reduce = results => from result in results
                                group result by result.Tag
                                into g
                                select new TossConnectedTagsIndex()
                                {
                                    Tag = g.Key,
                                    ConnectedTags = g.SelectMany(t => t.ConnectedTags).Distinct()
                                };
        }
    }
}
