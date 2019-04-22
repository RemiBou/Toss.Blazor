using MediatR;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toss.Server.Controllers;
using Toss.Server.Data;
using Toss.Server.Services;
using Toss.Shared.Tosses;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Tosses
{

    public class LastTossQueryHandlerTest : BaseTest
    {

        [Fact]
        public async Task last_returns_last_items_from_table_ordered_desc_by_createdon()
        {
            for (int i = 0; i < 11; i++)
            {
                FakeNow.Current = new DateTime(2017, 12, 31).AddDays(-i);
                await _mediator.Send(new TossCreateCommand()
                {
                    Content = "lorem #ipsum"
                });
            }
            await SaveAndWait();
            var res = await _mediator.Send(new TossLastQuery("ipsum"));

            Assert.Equal(10, res.Count());
            Assert.Null(res.FirstOrDefault(r => r.CreatedOn < new DateTime(2017, 12, 31).AddDays(-10)));
        }

        [Fact]
        public async Task returns_last_toss_paginated()
        {
            for (int i = 0; i < 31; i++)
            {
                FakeNow.Current = new DateTime(2017, 12, 31).AddDays(-i);
                await _mediator.Send(new TossCreateCommand()
                {
                    Content = "num"+i+" #test"
                });
            }
            await SaveAndWait();
            var res = await _mediator.Send(new TossLastQuery("test",1 ));
            Assert.Equal(10, res.Count());
            foreach(var toss in res)
            {
                Assert.InRange(toss.CreatedOn, new DateTime(2017, 12, 12), new DateTime(2017, 12, 21));
            }
            
        }

        [Fact]
        public async Task last_returns_toss_matching_hashtag()
        {

            for (int i = 0; i < 3; i++)
            {
                await _mediator.Send(new TossCreateCommand()
                {
                    Content = "lorem #ipsum #toto num" + i
                });
            }
            await _mediator.Send(new TossCreateCommand()
            {
                Content = "blabla #ipsum #tutu"
            });
            await SaveAndWait();
            var tosses = await _mediator.Send(
                new Toss.Shared.Tosses.TossLastQuery("toto"));
            Assert.Equal(3, tosses.Count());
            Assert.Null(tosses.FirstOrDefault(t => t.Content.Contains("#tutu")));
        }

        [Fact]
        public async Task last_returns_toss_content_truncated()
        {
            await _mediator.Send(new TossCreateCommand()
            {
                Content = string.Join(" ", Enumerable.Range(0, 100).Select(i => "lorem #ipsum #toto num")),

            });
            await SaveAndWait();

            var tosses = await _mediator.Send(
                new Toss.Shared.Tosses.TossLastQuery() { HashTag = "toto" });
            Assert.Equal(100, tosses.First().Content.Length);
        }


        [Fact]
        public async Task last_returns_toss_with_user_name()
        {
            await _mediator.Send(new TossCreateCommand()
            {
                Content = "blabla bla bla bla bla #test"

            });
            await SaveAndWait();
            var tosses = await _mediator.Send(
                new Toss.Shared.Tosses.TossLastQuery() { HashTag = "test" });
            Assert.Equal(serviceProviderInitializer.UserName, tosses.First().UserName);
        }

        [Fact]
        public async Task does_not_return_id_with_slash()
        {
            await _mediator.Send(new TossCreateCommand()
            {
                Content = "blabla bla bla bla bla #test"

            });
            await SaveAndWait();
            var tosses = await _mediator.Send(
                new Toss.Shared.Tosses.TossLastQuery() { HashTag = "test" });
            Assert.DoesNotContain("/", tosses.First().Id);

        }
    }
}
