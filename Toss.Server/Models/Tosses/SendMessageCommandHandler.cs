using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Raven.Client.Documents.Session;
using Toss.Server.Data;
using Toss.Server.Services;
using Toss.Shared.Tosses;

namespace Toss.Server.Models.Tosses
{
    class SendMessageCommandHandler : IRequestHandler<SendMessageCommand>
    {
        private readonly IAsyncDocumentSession _session;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RavenDBIdUtil _ravenDbIdUtil;
        public async Task<Unit> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            var conversation = await _session.LoadAsync<TossConversation>(_ravenDbIdUtil.GetRavenDbIdFromUrlId<TossEntity>(request.ConversationId));
            // we fail silently, there is no point in managing error when user hacked the desired behavior, this might change in the future
            if (conversation == null)
            {
                return Unit.Value;
            }
            var toss = await _session.LoadAsync<TossEntity>(conversation.TossId);
            var currentUser = _httpContextAccessor.HttpContext.User.UserId();
            //only conversation creator and toss creator can participate in a conversation
            if (currentUser != conversation.UserId || currentUser != toss.UserId)
            {
                return Unit.Value;
            }
            conversation.AddMessage(currentUser, request.Message);
            return Unit.Value;
        }
    }
}