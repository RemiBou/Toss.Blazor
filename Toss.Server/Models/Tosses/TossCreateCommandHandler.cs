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
using Toss.Server.Models.Tosses;
using Toss.Server.Services;
using Toss.Shared.Tosses;

namespace Toss.Server.Controllers
{
    public class TossCreateCommandHandler : IRequestHandler<TossCreateCommand>
    {
        private readonly IAsyncDocumentSession _session;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> userManager;
        private IStripeClient stripeClient;
        private IMediator mediator;
        private readonly INow now;

        public TossCreateCommandHandler(IAsyncDocumentSession session, IHttpContextAccessor httpContextAccessor, IStripeClient stripeClient, UserManager<ApplicationUser> userManager,
            IMediator mediator, INow now)
        {
            this.mediator = mediator;
            this.now = now;
            _session = session;
            _httpContextAccessor = httpContextAccessor;
            this.stripeClient = stripeClient;
            this.userManager = userManager;
        }

        public async Task<Unit> Handle(TossCreateCommand command, CancellationToken cancellationToken)
        {
            TossEntity toss;
            var user = _httpContextAccessor.HttpContext.User;
            if (!command.SponsoredDisplayedCount.HasValue)
                toss = new TossEntity(
                    command.Content,
                    user.UserId(),
                    now.Get());
            else
                toss = new SponsoredTossEntity(
                    command.Content,
                    user.UserId(),
                    now.Get(),
                    command.SponsoredDisplayedCount.Value);
            toss.UserName = user.Identity.Name;
            var matchCollection = TossCreateCommand.TagRegex.Matches(toss.Content);
            toss.Tags = matchCollection.Select(m => m.Groups[1].Value).ToList();
            toss.UserIp = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            await _session.StoreAsync(toss);
            if (command.SponsoredDisplayedCount.HasValue)
            {
                ApplicationUser applicationUser = (await userManager.GetUserAsync(user));
                var paymentResult = await stripeClient.Charge(command.StripeChargeToken, command.SponsoredDisplayedCount.Value * TossCreateCommand.CtsCostPerDisplay, "Payment for sponsored Toss #" + toss.Id, applicationUser.Email);
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
