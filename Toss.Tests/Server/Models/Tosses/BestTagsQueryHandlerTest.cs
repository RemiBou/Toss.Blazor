using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toss.Server.Services;
using Toss.Shared.Tosses;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Tosses
{
    public class BestTagsQueryHandlerTest : BaseTest
    {
        [Fact]
        public async Task on_new_toss_tags_are_on_best_tags_result()
        {
            await _mediator.Send(
                   new TossCreateCommand()
                   {
                       Content = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum #test"
                   });

            var res = await _mediator.Send(new BestTagsQuery());

            var single = Assert.Single(res);
            Assert.Equal(1, single.CountLastMonth);
            Assert.Equal("test", single.Tag);
        }

        [Fact]
        public async Task best_tags_only_give_best_50_tags()
        {
            //#toss1 - 5 willbe added once, #toss6 - 55 twice
            for (int i = 1; i <= 55; i++)
            {
                if (i <= 5)
                {
                    await _mediator.Send(
                       new TossCreateCommand()
                       {
                           Content = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum #test" + i
                       });
                }
                else
                {
                    await _mediator.Send(
                      new TossCreateCommand()
                      {
                          Content = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum #test" + i
                      });
                    await _mediator.Send(
                      new TossCreateCommand()
                      {
                          Content = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum #test" + i
                      });
                }
            }
            await SaveAndWait();
            var res = await _mediator.Send(new BestTagsQuery());
            Assert.Equal(50, res.Count);
            for (int i = 6; i <= 55; i++)
            {
                var tagRef = res.FirstOrDefault(r => r.Tag == "test" + i);
                Assert.NotNull(tagRef);
                Assert.Equal(2, tagRef.CountLastMonth);
            }
        }


        [Fact]
        public async Task best_tags_only_count_last_30_days()
        {
            for (int i = 0; i <= 32; i++)
            {
                FakeNow.Current = DateTimeOffset.Now.AddDays(-i);
                await _mediator.Send(
                   new TossCreateCommand()
                   {
                       Content = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum #test"
                   });
            }
            await SaveAndWait();

            FakeNow.Current = DateTimeOffset.Now;
            var res = await _mediator.Send(new BestTagsQuery());

            Assert.Equal(30, res.First().CountLastMonth);
        }
    }
}
