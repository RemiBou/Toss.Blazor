using Raven.Client.Documents.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toss.Server.Models;

namespace Toss.Server.Data.Indexes
{
    public class Toss_ConnectedTags : AbstractMultiMapIndexCreationTask<TossConnectedTagsIndex>
    {
        public Toss_ConnectedTags()
        {
            AddMap< TossEntity>(tosses => from toss in tosses
                            from tag in toss.Tags.Distinct()
                            from tag2 in toss.Tags.Distinct()
                            where tag != tag2
                            select new TossConnectedTagsIndex()
                            {
                                Tag1 = tag,
                                Tag2 = tag2,
                                Count = 1
                            });
            AddMap<ApplicationUser>(users => from user in users
                                         from tag in user.Hashtags.Distinct()
                                         from tag2 in user.Hashtags.Distinct()
                                         where tag != tag2
                                         select new TossConnectedTagsIndex()
                                         {
                                             Tag1 = tag,
                                             Tag2 = tag2,
                                             Count = 1
                                         });
            Reduce = results => from result in results
                                group result by new { result.Tag1, result.Tag2 }
                                into g
                                select new TossConnectedTagsIndex()
                                {
                                    Tag1 = g.Key.Tag1,
                                    Tag2 = g.Key.Tag2,
                                    Count = g.Sum(c => c.Count)
                                    
                                };
        }
    }
}
