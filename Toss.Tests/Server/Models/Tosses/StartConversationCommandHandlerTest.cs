using System.Threading.Tasks;
using Toss.Shared.Tosses;
using Toss.Tests.Infrastructure;
using Xunit;
using System.Linq;
using System;

namespace Toss.Tests.Server.Models.Tosses
{
    public class StartConversationCommandHandlerTest : BaseTest
    {
        [Fact]
        async Task cannot_create_two_discussion_on_same_toss()
        {
            TossLastQueryItem toss = await CreateTossAndConversation();
            await Assert.ThrowsAnyAsync<Exception>(() => _mediator.Send(new StartConversationCommand(toss.Id)));
        }

      
        [Fact]
        async Task cannot_create_discussion_on_our_toss()
        {
            await _mediator.Send(new TossCreateCommand("test bla bla bla bla #test"));
            await SaveAndWait();
            var toss = (await _mediator.Send(new TossLastQuery("test"))).First();
            await SaveAndWait();
            await Assert.ThrowsAnyAsync<Exception>(() => _mediator.Send(new StartConversationCommand(toss.Id)));
        }

        [Fact]
        async Task created_conversation_returned_by_query_for_discussion_creator()
        {
            var toss = await CreateTossAndConversation();

            var res = await _mediator.Send(new TossConversationQuery(toss.Id));

            Assert.Single(res.Conversations);
        }

        [Fact]
        async Task created_conversation_returns_creator_username()
        {
            var toss = await CreateTossAndConversation();

            var res = await _mediator.Send(new TossConversationQuery(toss.Id));

            Assert.Equal("discusioncreator", res.Conversations.First().CreatorUserName);
        }

        [Fact]
        async Task created_conversation_returned_by_query_for_toss_creator()
        {
            var toss = await CreateTossAndConversation();
            //create an other conversation for same toss
            await CreateNewUserIfNotExists("discusioncreator2");
            await _mediator.Send(new StartConversationCommand(toss.Id));
            await SaveAndWait();
            //this will switch to first user
            await CreateTestUser();
            var res = await _mediator.Send(new TossConversationQuery(toss.Id));
            Assert.Equal(2,res.Conversations.Count);
        }


        [Fact]
        async Task created_converstion_not_return_by_query_for_other_user()
        {
            var toss = await CreateTossAndConversation();
            //this will switch to first user
            await CreateNewUserIfNotExists("hackermen");
            var res = await _mediator.Send(new TossConversationQuery(toss.Id));

            Assert.Empty(res.Conversations);
        }


        private async Task<TossLastQueryItem> CreateTossAndConversation()
        {
            await _mediator.Send(new TossCreateCommand("test bla bla bla bla #test"));
            await SaveAndWait();
            var toss = (await _mediator.Send(new TossLastQuery("test"))).First();
            await CreateNewUserIfNotExists("discusioncreator");
            await SaveAndWait();
            await _mediator.Send(new StartConversationCommand(toss.Id));
            await SaveAndWait();
            return toss;
        }


    }
}