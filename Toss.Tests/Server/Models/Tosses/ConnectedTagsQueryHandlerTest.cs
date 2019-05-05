using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Toss.Shared.Account;
using Toss.Shared.Tosses;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Tosses
{
    public class ConnectedTagsQueryHandlerTest : BaseTest
    {
        [Fact]
        public async Task when_two_tags_in_same_toss_connected_tag_returns_it_bi_directionnal()
        {
            await _mediator.Send(new TossCreateCommand("bla bla bla bla bla bla bla #test #test2"));
            await SaveAndWait();

            var res =await  _mediator.Send(new ConnectedTagsQuery("test"));

            Assert.NotNull(res);
            Assert.Single(res.Hashtags);
            Assert.Equal("test2", res.Hashtags[0]);

            res = await _mediator.Send(new ConnectedTagsQuery("test2"));

            Assert.NotNull(res);
            Assert.Single(res.Hashtags);
            Assert.Equal("test", res.Hashtags[0]);
        }

        [Fact]
        public async Task when_two_tags_added_by_one_person_returns_it_bi_directionnal()
        {
            await _mediator.Send(new AddHashtagCommand("test"));
            await _mediator.Send(new AddHashtagCommand("test2"));
            await SaveAndWait();
            var res = await _mediator.Send(new ConnectedTagsQuery("test"));

            Assert.NotNull(res);
            Assert.Single(res.Hashtags);
            Assert.Equal("test2", res.Hashtags[0]);

            res = await _mediator.Send(new ConnectedTagsQuery("test2"));

            Assert.NotNull(res);
            Assert.Single(res.Hashtags);
            Assert.Equal("test", res.Hashtags[0]);
        }

        [Fact]
        public async Task when_a_user_removes_a_tag_do_not_return_it_anymore()
        {
            await _mediator.Send(new AddHashtagCommand("test"));
            await _mediator.Send(new AddHashtagCommand("test2"));
            await SaveAndWait();
            await _mediator.Send(new RemoveHashTagCommand("test2"));
            await SaveAndWait();
            var res = await _mediator.Send(new ConnectedTagsQuery("test"));

            Assert.NotNull(res);
            Assert.Empty(res.Hashtags);

            res = await _mediator.Send(new ConnectedTagsQuery("test2"));

            Assert.NotNull(res);
            Assert.Empty(res.Hashtags);
        }
    }
}
