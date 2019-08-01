using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Raven.Client.Documents.Session;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toss.Server.Data;
using Toss.Server.Models;
using Toss.Server.Models.Account;
using Toss.Server.Models.Tosses;
using Toss.Server.Services;
using Toss.Shared.Tosses;

namespace Toss.Server.Controllers
{
    public class TossCreateCommandHandler : IRequestHandler<TossCreateCommand>
    {
        private readonly IAsyncDocumentSession _session;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IStripeClient stripeClient;
        private readonly IMediator mediator;
        private readonly INow now;

        private readonly IHttpContextAccessor httpContextAccessor;

        public TossCreateCommandHandler(IAsyncDocumentSession session, UserManager<ApplicationUser> userManager, IStripeClient stripeClient, IMediator mediator, INow now, IHttpContextAccessor httpContextAccessor)
        {
            _session = session;
            this.userManager = userManager;
            this.stripeClient = stripeClient;
            this.mediator = mediator;
            this.now = now;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(TossCreateCommand command, CancellationToken cancellationToken)
        {
            TossEntity toss;
            var user = await mediator.Send(new CurrentUserQuery());
            if (!command.SponsoredDisplayedCount.HasValue)
                toss = new TossEntity(
                    command.Content,
                    user.Id,
                    now.Get());
            else
                toss = new SponsoredTossEntity(
                    command.Content,
                    user.Id,
                    now.Get(),
                    command.SponsoredDisplayedCount.Value);
            toss.UserName = user.UserName;
            var matchCollection = TossCreateCommand.TagRegex.Matches(toss.Content);
            toss.Tags = matchCollection.Select(m => m.Groups[1].Value).ToList();
            toss.UserIp = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            await _session.StoreAsync(toss);
            if (command.SponsoredDisplayedCount.HasValue)
            {

                var paymentResult = await stripeClient.Charge(command.StripeChargeToken, command.SponsoredDisplayedCount.Value * TossCreateCommand.CtsCostPerDisplay, "Payment for sponsored Toss #" + toss.Id, user.Email);
                if (!paymentResult)
                {
                    _session.Delete(toss.Id);
                    throw new InvalidOperationException("Payment error on sponsored Toss ");
                }

            }
            await mediator.Publish(new TossCreated(toss));
            return Unit.Value;
        }
    }
}
