using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Threading;
using System.Threading.Tasks;
using Toss.Server.Data;
using Toss.Server.Models.Account;
using Toss.Shared.Tosses;

namespace Toss.Server.Models.Tosses
{
    public class StartConversationCommandHandler : IRequestHandler<StartConversationCommand>
    {
        private readonly IAsyncDocumentSession _session;
        private readonly RavenDBIdUtil _ravenDbIdUtil;
        private readonly IMediator mediator;

        public StartConversationCommandHandler(IAsyncDocumentSession session, RavenDBIdUtil ravenDbIdUtil, IMediator mediator)
        {
            _session = session;
            _ravenDbIdUtil = ravenDbIdUtil;
            this.mediator = mediator;
        }

        public async Task<Unit> Handle(StartConversationCommand request, CancellationToken cancellationToken)
        {
            var toss = await _session.LoadAsync<TossEntity>(_ravenDbIdUtil.GetRavenDbIdFromUrlId<TossEntity>(request.TossId));
            // we fail silently, there is no point in managing error when user hacked the desired behavior, this might change in the future
            if (toss == null)
            {
                throw new InvalidOperationException($"Toss does not exists : {request.TossId}");
            }
            var currentUser = await mediator.Send(new CurrentUserQuery());
            if (currentUser.Id == toss.UserId)
            {
                throw new InvalidOperationException($"Cannot create conversation when toss creator : {request.TossId}");
            }

            if (await _session.Query<TossConversation>().AnyAsync(c => c.CreatorUserId == currentUser.Id && c.TossId == toss.Id))
            {
                throw new InvalidOperationException($"Conversation already exists. User : {currentUser}, Toss: {toss.Id}");
            }

            TossConversation conversation = new TossConversation(toss, currentUser);
            await _session.StoreAsync(conversation);
            await mediator.Publish(new ConversationStarted(conversation));
            return Unit.Value;
        }
    }
}