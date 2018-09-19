using MediatR;
using System;
using System.Threading.Tasks;
using Toss.Server.Data;
using Toss.Server.Services;
using Toss.Shared.Tosses;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Tosses
{
    public class CreateTossCommandHandlerTest : BaseIntegrationTest
    {   private ICosmosDBTemplate<TossEntity> tossTemplate;
        public CreateTossCommandHandlerTest()
        {
            tossTemplate = TestFixture.GetInstance<ICosmosDBTemplate<TossEntity>>();
        }
        [Fact]
        public async Task create_setup_username_to_current_user()
        {
            await TestFixture.GetInstance<IMediator>().Send(
                new TossCreateCommand()
                {
                    Content = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum"
                });

            var toss = await (await tossTemplate.CreateDocumentQuery()).GetFirstOrDefault();

            Assert.Equal(TestFixture.UserName, toss.UserName);

        }
        [Fact]
        public async Task create_setup_date_of_post_to_today()
        {
            var now = DateTimeOffset.Now.AddMinutes(-1);
            await TestFixture.GetInstance<IMediator>().Send(new TossCreateCommand()
            {
                Content = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum"
            });
            var now2 = DateTimeOffset.Now.AddMinutes(1);

            var toss = await (await tossTemplate.CreateDocumentQuery()).GetFirstOrDefault();

            Assert.True(toss.CreatedOn >= now && toss.CreatedOn <= now2); 

        }

        [Fact]
        public async Task create_insert_item_in_cosmos()
        {
            await TestFixture.GetInstance<IMediator>().Send(new TossCreateCommand()
            {
                Content = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum"
            });
            

            var toss = await (await tossTemplate.CreateDocumentQuery()).GetFirstOrDefault();

            Assert.NotNull(toss);
            Assert.Equal("lorem ipsum lorem ipsum lorem ipsum lorem ipsum", toss.Content);
        }

        [Fact]
        public async Task create_when_display_count_creates_SponsoredToss()
        {
            await TestFixture.GetInstance<IMediator>().Send(new TossCreateCommand()
            {
                Content = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum",
                SponsoredDisplayedCount = 1000
            });

            var toss = await (await tossTemplate.CreateDocumentQuery()).GetFirstOrDefault();

            Assert.NotNull(toss);
            var sponsored = Assert.IsAssignableFrom<SponsoredTossEntity>(toss);
            Assert.Equal(1000, sponsored.DisplayedCount);
            Assert.Equal(1000, sponsored.DisplayedCountBought);
        }

        [Fact]
        public async Task create_when_sponsored_payment_succeed_return_ok()
        {
            await TestFixture.GetInstance<IMediator>().Send(new TossCreateCommand()
            {
                Content = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum",
                SponsoredDisplayedCount = 1000,
                StripeChargeToken = "AAA"
            });
            
            var toss = await (await tossTemplate.CreateDocumentQuery()).GetFirstOrDefault();
            //_mockStripe.Verify(m => m.Charge("AAA", command.SponsoredDisplayedCount.Value * TossCreateCommand.CtsCostPerDisplay, It.Is<string>(s => s.Contains(toss.Id)), _m.ApplicationUser.Email));
        }

        [Fact]
        public async Task create_when_payment_fails_delete_toss_throw_error()
        {
            FakeStripeClient.NextCallFails = true;
            await Assert.ThrowsAsync<InvalidOperationException>(async () => 
                await TestFixture.GetInstance<IMediator>().Send(new TossCreateCommand()
                {
                    Content = "lorem ipsum erzer zer zer ze rze rezr zer",
                    SponsoredDisplayedCount = 1000,
                    StripeChargeToken = "AAA"
                }));
            var toss = await (await tossTemplate.CreateDocumentQuery()).GetFirstOrDefault();

            Assert.Null(toss);


        }

        

    }
}
