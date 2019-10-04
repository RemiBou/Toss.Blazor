using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents.Session;
using System;
using System.Threading;
using System.Threading.Tasks;
using Toss.Server.Data;
using Toss.Server.Extensions;
using Toss.Server.Services;

namespace Toss.Server.Models.Tosses
{
    public class SendMailOnConversationStarted : MediatR.INotificationHandler<ConversationStarted>
    {
        private readonly IEmailSender _emailSender;
        private readonly IAsyncDocumentSession documentSession;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUrlHelper _urlHelper;
        private readonly RavenDBIdUtil ravenDBIdUtil;

        public SendMailOnConversationStarted(IEmailSender emailSender, IAsyncDocumentSession documentSession, IHttpContextAccessor httpContextAccessor, IUrlHelper urlHelper, RavenDBIdUtil ravenDBIdUtil)
        {
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            this.documentSession = documentSession ?? throw new ArgumentNullException(nameof(documentSession));
            this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _urlHelper = urlHelper ?? throw new ArgumentNullException(nameof(urlHelper));
            this.ravenDBIdUtil = ravenDBIdUtil ?? throw new ArgumentNullException(nameof(ravenDBIdUtil));
        }

        public async Task Handle(ConversationStarted notification, CancellationToken cancellationToken)
        {
            var toss = await documentSession.LoadAsync<TossEntity>(notification.Conversation.TossId);
            var users = await documentSession.LoadAsync<ApplicationUser>(new[] { notification.Conversation.CreatorUserId, toss.UserId });
            await _emailSender.SendNewConversationAsync(
                users[toss.UserId].Email,
                users[toss.UserId].UserName,
                users[notification.Conversation.CreatorUserId].UserName,
                _urlHelper.TossLink(ravenDBIdUtil.GetUrlId(toss.Id), httpContextAccessor.HttpContext.Request.Scheme)
                );
        }
    }
}
