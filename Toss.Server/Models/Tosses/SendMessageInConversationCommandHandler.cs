using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Raven.Client.Documents.Session;
using Toss.Server.Data;
using Toss.Server.Models.Account;
using Toss.Server.Services;
using Toss.Shared.Tosses;

namespace Toss.Server.Models.Tosses
{
    class SendMessageInConversationCommandHandler : IRequestHandler<SendMessageInConversationCommand>
    {
        private readonly IAsyncDocumentSession _session;
        private readonly RavenDBIdUtil _ravenDbIdUtil;

        private readonly IMediator mediator;
        private readonly INow now;

        public SendMessageInConversationCommandHandler(IAsyncDocumentSession session, RavenDBIdUtil ravenDbIdUtil, IMediator mediator, INow now)
        {
            _session = session;
            _ravenDbIdUtil = ravenDbIdUtil;
            this.mediator = mediator;
            this.now = now;
        }

        public async Task<Unit> Handle(SendMessageInConversationCommand request, CancellationToken cancellationToken)
        {
            var conversation = await _session.LoadAsync<TossConversation>(_ravenDbIdUtil.GetRavenDbIdFromUrlId<TossConversation>(request.ConversationId));
            // we fail silently, there is no point in managing error when user hacked the desired behavior, this might change in the future
            if (conversation == null)
            {
                throw new ApplicationException($"Conversation {conversation.Id} does not exists");
            }
            var toss = await _session.LoadAsync<TossEntity>(conversation.TossId);
            var currentUser = await mediator.Send(new CurrentUserQuery());
            //only conversation creator and toss creator can participate in a conversation
            if (conversation.CanSendMessage(toss, currentUser))
            {
                throw new ApplicationException($"{currentUser.Id} can't send message in {conversation.Id}");
            }
            conversation.AddMessage(currentUser, request.Message, this.now.Get());
            return Unit.Value;
        }
    }
}