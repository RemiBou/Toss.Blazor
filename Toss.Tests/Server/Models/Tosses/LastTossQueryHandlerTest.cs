using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toss.Server.Controllers;
using Toss.Server.Data;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Tosses
{
    
    public class LastTossQueryHandlerTest : BaseCosmosTest
    {
        private ICosmosDBTemplate<TossEntity> tossTemplate;
        public LastTossQueryHandlerTest()
        {
            tossTemplate = TestFixture.GetInstance<ICosmosDBTemplate<TossEntity>>();
            
        }

        [Fact]
        public async Task last_returns_last_items_from_table_ordered_desc_by_createdon()
        {
            for (int i = 0; i < 60; i++)
            {
                await tossTemplate.Insert(new TossEntity()
                {
                    Content = "lorem #ipsum",
                    CreatedOn = new DateTime(2017, 12, 31).AddDays(-i),
                    UserName = "usernametest"
                });
            }

            var res = await _mediator.Send(new Toss.Shared.Tosses.TossLastQuery("ipsum")) ;

            Assert.Equal(50, res.Count());
            Assert.Null(res.FirstOrDefault(r => r.CreatedOn < new DateTime(2017, 12, 31).AddDays(-50)));
        }

        [Fact]
        public async Task last_returns_toss_matching_hashtag()
        {

            for (int i = 0; i < 3; i++)
            {
                await tossTemplate.Insert(new TossEntity()
                {
                    Content = "lorem #ipsum #toto num" + i,
                    CreatedOn = DateTimeOffset.Now,
                    UserName = "usernametest"
                });
            }
            await tossTemplate.Insert(new TossEntity()
            {
                Content = "blabla #ipsum #tutu",
                CreatedOn = DateTimeOffset.Now,
                UserName = "usernametest"
            });


            var tosses = await _mediator.Send(
                new Toss.Shared.Tosses.TossLastQuery() { HashTag = "toto" });
            Assert.Equal(3, tosses.Count());
            Assert.Null(tosses.FirstOrDefault(t => t.Content.Contains("#tutu")));
        }
    }
}
