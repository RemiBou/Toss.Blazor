using MediatR;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toss.Server.Data;
using Toss.Server.Models.Tosses;
using Toss.Server.Services;
using Toss.Shared.Tosses;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Tosses
{
    public class TossListAdminQueryHandlerTest : BaseTest
    {
        public TossListAdminQueryHandlerTest()
        {
        }

        [Fact]
        public async Task Handle_returns_toss_count()
        {
            for (int i = 0; i < 59; i++)
            {
                await _mediator.Send(new TossCreateCommand() { Content = "test" });
            }
            await SaveAndWait();
            var res = await _mediator.Send(new Toss.Shared.Tosses.TossListAdminQuery(), new System.Threading.CancellationToken());

            Assert.Equal(59, res.Count);
        }

        [Fact]
        public async Task Handle_return_last_items()
        {
            for (int i = 0; i < 27; i++)
            {
                FakeNow.Current = DateTimeOffset.Now.AddDays(-i);
                await _mediator.Send(new TossCreateCommand() { Content = "test" });
            }
            await SaveAndWait();

            var res = await _mediator.Send(new Toss.Shared.Tosses.TossListAdminQuery(15, null));

            Assert.Equal(15, res.Result.Count());
            Assert.Null(res.Result.FirstOrDefault(r => r.CreatedOn <= DateTimeOffset.Now.AddDays(-15)));
        }

        [Fact]
        public async Task Handle_filter_on_max_date()
        {
            for (int i = 0; i < 27; i++)
            {
                FakeNow.Current = DateTimeOffset.Now.AddDays(-i);
                await _mediator.Send(new TossCreateCommand() { Content = "test" });
            }
            await SaveAndWait();
            var res = await _mediator.Send(new TossListAdminQuery(15, DateTimeOffset.Now.AddDays(-14)));

            Assert.Equal(13, res.Result.Count());
            Assert.Null(res.Result.FirstOrDefault(r => r.CreatedOn > DateTimeOffset.Now.AddDays(-14)));
        }

        [Fact]
        public async Task Handle_map_fields()
        {

            FakeNow.Current = DateTimeOffset.Now;
            await _mediator.Send(new TossCreateCommand() { Content = "lorem ipsum" });
            await SaveAndWait();
            var res = await _mediator.Send(new Toss.Shared.Tosses.TossListAdminQuery(15, null));

            var first = res.Result.FirstOrDefault();

            Assert.Equal("lorem ipsum", first.Content);
            Assert.Equal(FakeNow.Current, first.CreatedOn);
            Assert.Equal("username", first.UserName);
            Assert.NotNull(first.Id);
        }
    }
}
