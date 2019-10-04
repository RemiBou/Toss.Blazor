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
    public class SendMessageInConversationCommandHandlerTest : BaseTest
    {


        [Fact]
        async Task send_message_if_not_toss_creator_or_conversation_creator_throw_exception()
        {
            var conv = await CreateTossAndConversation();
            await CreateNewUserIfNotExists("hackermen");
            await Assert.ThrowsAnyAsync<Exception>(() =>
                _mediator.Send(new SendMessageInConversationCommand(conv.Id, "This is a test")));
        }

        [Fact]
        async Task send_message_then_appear_in_query_by_conversation_creator()
        {
            var conv = await CreateTossAndConversation();
            await _mediator.Send(new SendMessageInConversationCommand(conv.Id, "This is a test"));
            await SaveAndWait();
            var res = await _mediator.Send(new MessageInConversationQuery(conv.Id));

            Assert.Single(res.Messages);

        }


        [Fact]
        async Task send_message_then_appear_in_query_by_toss_creator()
        {
            var conv = await CreateTossAndConversation();
            await _mediator.Send(new SendMessageInConversationCommand(conv.Id, "This is a test"));
            await SaveAndWait();
            await CreateTestUser();
            await SaveAndWait();
            var res = await _mediator.Send(new MessageInConversationQuery(conv.Id));

            Assert.Single(res.Messages);
        }

        [Fact]
        async Task send_message_then_appear_in_query_with_fields()
        {
            FakeNow.Current = new DateTimeOffset(2018, 1, 1, 1, 1, 1, 1, TimeSpan.Zero);
            var conv = await CreateTossAndConversation();
            await _mediator.Send(new SendMessageInConversationCommand(conv.Id, "This is a test"));
            await SaveAndWait();
            var res = await _mediator.Send(new MessageInConversationQuery(conv.Id));

            MessageInConversationQueryResultItem message = res.Messages.First();
            Assert.Equal("discusioncreator", message.UserName);
            Assert.Equal(FakeNow.Current, message.CreatedOn);
            Assert.Equal("This is a test", message.Content);
        }


        [Fact]
        async Task send_message_then_conversation_query_returns_message_count()
        {
            await _mediator.Send(new TossCreateCommand("test bla bla bla bla #test"));
            await SaveAndWait();
            var toss = (await _mediator.Send(new TossLastQuery("test"))).First();
            await CreateNewUserIfNotExists("discusioncreator");
            await SaveAndWait();
            await _mediator.Send(new StartConversationCommand(toss.Id));
            await SaveAndWait();
            var conversation = await _mediator.Send(new TossConversationQuery(toss.Id));
            await _mediator.Send(new SendMessageInConversationCommand(conversation.Conversations.First().Id, "This is a test"));
            await SaveAndWait();

            var res = await _mediator.Send(new TossConversationQuery(toss.Id));

            Assert.Equal(1, res.Conversations.First().MessageCount);
        }


        private async Task<TossConversationQueryResultItem> CreateTossAndConversation()
        {
            await _mediator.Send(new TossCreateCommand("test bla bla bla bla #test"));
            await SaveAndWait();
            var toss = (await _mediator.Send(new TossLastQuery("test"))).First();
            await CreateNewUserIfNotExists("discusioncreator");
            await SaveAndWait();
            await _mediator.Send(new StartConversationCommand(toss.Id));
            await SaveAndWait();
            var conversation = await _mediator.Send(new TossConversationQuery(toss.Id));
            return conversation.Conversations.First();
        }

    }
}
