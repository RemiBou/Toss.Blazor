using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
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
        private readonly ICosmosDBTemplate<TossEntity> _dbTemplate;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> userManager;
        private IStripeClient stripeClient;
        private IMediator mediator;

        public TossCreateCommandHandler(ICosmosDBTemplate<TossEntity> cosmosTemplate, IHttpContextAccessor httpContextAccessor, IStripeClient stripeClient, UserManager<ApplicationUser> userManager,
            IMediator mediator)
        {
            this.mediator = mediator;
            _dbTemplate = cosmosTemplate;
            _httpContextAccessor = httpContextAccessor;
            this.stripeClient = stripeClient;
            this.userManager = userManager;
        }

        public async Task<Unit> Handle(TossCreateCommand command, CancellationToken cancellationToken)
        {
            TossEntity toss;
            if (!command.SponsoredDisplayedCount.HasValue)
                toss = new TossEntity(
                    command.Content,
                    _httpContextAccessor.HttpContext.User.Identity.Name,
                    DateTimeOffset.Now);
            else
                toss = new SponsoredTossEntity(
                    command.Content,
                    _httpContextAccessor.HttpContext.User.Identity.Name,
                    DateTimeOffset.Now,
                    command.SponsoredDisplayedCount.Value);
            toss.Id = await _dbTemplate.Insert(toss);
            if (command.SponsoredDisplayedCount.HasValue)
            {
                ApplicationUser applicationUser = (await userManager.GetUserAsync(_httpContextAccessor.HttpContext.User));
                var paymentResult = await stripeClient.Charge(command.StripeChargeToken, command.SponsoredDisplayedCount.Value * TossCreateCommand.CtsCostPerDisplay, "Payment for sponsored Toss #" + toss.Id, applicationUser.Email);
                if (!paymentResult)
                {
                    await _dbTemplate.Delete(toss.Id);
                    throw new InvalidOperationException("Payment error on sponsored Toss ");
                }

            }
            await mediator.Publish(new TossCreated(toss.Id));
            return Unit.Value;
        }
    }
}
