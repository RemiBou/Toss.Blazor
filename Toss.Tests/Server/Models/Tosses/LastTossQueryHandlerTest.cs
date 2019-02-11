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
        private IAsyncDocumentSession _session;
        public LastTossQueryHandlerTest()
        {
            _session = serviceProviderInitializer.GetInstance<IAsyncDocumentSession>();

        }

        [Fact]
        public async Task last_returns_last_items_from_table_ordered_desc_by_createdon()
        {
            for (int i = 0; i < 60; i++)
            {
                FakeNow.Current = new DateTime(2017, 12, 31).AddDays(-i);
                await _mediator.Send(new TossCreateCommand()
                {
                    Content = "lorem #ipsum"
                });
            }
            await SaveAndWait();
            var res = await _mediator.Send(new TossLastQuery("ipsum"));

            Assert.Equal(50, res.Count());
            Assert.Null(res.FirstOrDefault(r => r.CreatedOn < new DateTime(2017, 12, 31).AddDays(-50)));
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
                new Toss.Shared.Tosses.TossLastQuery() { HashTag = "toto" });
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
    }
}
