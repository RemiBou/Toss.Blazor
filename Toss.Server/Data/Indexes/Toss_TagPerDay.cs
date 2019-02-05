using System.Linq;
using Raven.Client.Documents.Indexes;
using Toss.Server.Models.Tosses;

namespace Toss.Server.Data.Indexes
{
    public class Toss_TagPerDay : AbstractIndexCreationTask<TossEntity, TagByDayIndex>
    {
        public Toss_TagPerDay()
        {
            
            Map = tosses => from toss in tosses
                            from tag in toss.Tags
                            select new TagByDayIndex()
                            {
                                Tag = tag,
                                CreatedOn = toss.CreatedOn.Date,
                                Count = 1
                            };
            Reduce = results => from result in results
                                group result by new { result.Tag, result.CreatedOn }
                                into g
                                select new TagByDayIndex()
                                {
                                    Tag = g.Key.Tag,
                                    CreatedOn = g.Key.CreatedOn,
                                    Count = g.Sum(i => i.Count)
                                };
        }
    }
}