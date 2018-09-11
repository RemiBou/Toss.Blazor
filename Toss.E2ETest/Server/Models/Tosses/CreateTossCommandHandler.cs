using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toss.Server.Controllers;
using Toss.Server.Data;
using Toss.Server.Services;
using Toss.Shared;
using Toss.Shared.Tosses;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Tosses
{
    [Collection("CosmosDBFixture Collection")]
    public class CreateTossCommandHandlerTest : BaseCosmosTest
    {
        private CommonMocks<TossController> _m;
        private Mock<IStripeClient> _mockStripe;
        private ICosmosDBTemplate<TossEntity> tossTemplate;
        private TossCreateCommandHandler _sut;
        public CreateTossCommandHandlerTest(CosmosDBFixture fixture):base(fixture)
        {
            _m = new CommonMocks<TossController>();
            _mockStripe = new Mock<IStripeClient>();
            tossTemplate = GetTemplate<TossEntity>();
            _sut = new TossCreateCommandHandler(
                tossTemplate,
                _m.HttpContextAccessor.Object,
                _mockStripe.Object,
                _m.UserManager.Object);
        }
        [Fact]
        public async Task create_setup_username_to_current_user()
        {

            var command = new TossCreateCommand()
            {
                Content = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum"
            };
            var res = await _sut.Handle(command, new System.Threading.CancellationToken());

            var toss = await (await tossTemplate.CreateDocumentQuery()).GetFirstOrDefault();

            Assert.Equal(_m.ApplicationUser.UserName, toss.UserName);

        }
        [Fact]
        public async Task create_setup_date_of_post_to_today()
        {

            var command = new TossCreateCommand()
            {
                Content = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum"
            };
            var now = DateTimeOffset.Now.AddMinutes(-1);
            var res = await _sut.Handle(command, new System.Threading.CancellationToken());

            var now2 = DateTimeOffset.Now.AddMinutes(1);

            var toss = await (await tossTemplate.CreateDocumentQuery()).GetFirstOrDefault();

            Assert.True(toss.CreatedOn >= now && toss.CreatedOn <= now2); 

        }

        [Fact]
        public async Task create_insert_item_in_cosmos()
        {
            var command = new TossCreateCommand()
            {
                Content = "lorem ipsum"
            };

            await _sut.Handle(command, new System.Threading.CancellationToken());


            var toss = await (await tossTemplate.CreateDocumentQuery()).GetFirstOrDefault();

            Assert.NotNull(toss);
            Assert.Equal("lorem ipsum", toss.Content);
        }

        [Fact]
        public async Task create_when_display_count_creates_SponsoredToss()
        {
            var command = new TossCreateCommand()
            {
                Content = "lorem ipsum",
                SponsoredDisplayedCount = 1000
            };
            _mockStripe.Setup(m => m.Charge(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true)) ;
            await _sut.Handle(command, new System.Threading.CancellationToken());


            var toss = await (await tossTemplate.CreateDocumentQuery()).GetFirstOrDefault();

            Assert.NotNull(toss);
            var sponsored = Assert.IsAssignableFrom<SponsoredTossEntity>(toss);
            Assert.Equal(1000, sponsored.DisplayedCount);
            Assert.Equal(1000, sponsored.DisplayedCountBought);
        }

        [Fact]
        public async Task create_when_sponsored_payment_succeed_return_ok()
        {
            var command = new TossCreateCommand()
            {
                Content = "lorem ipsum",
                SponsoredDisplayedCount = 1000,
                StripeChargeToken = "AAA"
            };
            _mockStripe.Setup(m => m.Charge(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));
            await _sut.Handle(command, new System.Threading.CancellationToken());
            var toss = await (await tossTemplate.CreateDocumentQuery()).GetFirstOrDefault();
            _mockStripe.Verify(m => m.Charge("AAA", command.SponsoredDisplayedCount.Value * TossCreateCommand.CtsCostPerDisplay, It.Is<string>(s => s.Contains(toss.Id)), _m.ApplicationUser.Email));
        }

        [Fact]
        public async Task create_when_payment_fails_delete_toss_throw_error()
        {
            var command = new TossCreateCommand()
            {
                Content = "lorem ipsum",
                SponsoredDisplayedCount = 1000,
                StripeChargeToken = "AAA"
            };
            _mockStripe.Setup(m => m.Charge(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(false));
            await _sut.Handle(command, new System.Threading.CancellationToken());
            var toss = await (await tossTemplate.CreateDocumentQuery()).GetFirstOrDefault();

            Assert.Null(toss);


        }


    }
}
