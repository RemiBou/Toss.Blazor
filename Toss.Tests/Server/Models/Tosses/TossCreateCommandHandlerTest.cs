using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Linq;
using System.Threading.Tasks;
using Toss.Server.Data;
using Toss.Server.Models.Tosses;
using Toss.Server.Services;
using Toss.Shared.Tosses;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Tosses
{
    public class TossCreateCommandHandlerTest : BaseTest
    {
        private readonly IAsyncDocumentSession _session;
        public TossCreateCommandHandlerTest()
        {
            _session = serviceProviderInitializer.GetInstance<IAsyncDocumentSession>();
        }
        [Fact]
        public async Task create_setup_username_to_current_user()
        {
            await _mediator.Send(
                new TossCreateCommand()
                {
                    Content = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum"
                });

            await SaveAndWait();
            var toss = await _session.Query<TossEntity>().FirstOrDefaultAsync();

            Assert.Equal(serviceProviderInitializer.ClaimPrincipal.UserId(), toss.UserId);
            Assert.Equal(serviceProviderInitializer.ClaimPrincipal.Identity.Name, toss.UserName);

        }
        [Fact]
        public async Task create_setup_date_of_post_to_today()
        {
            FakeNow.Current = new DateTimeOffset(2018, 1, 1, 1, 1, 1, 1, TimeSpan.Zero);
            await _mediator.Send(new TossCreateCommand()
            {
                Content = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum"
            });


            await SaveAndWait();

            var toss = await _session.Query<TossEntity>().FirstOrDefaultAsync();

            Assert.True(toss.CreatedOn == FakeNow.Current);

        }

        [Fact]
        public async Task create_insert_item_in_cosmos()
        {
            await _mediator.Send(new TossCreateCommand()
            {
                Content = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum"
            });


            await SaveAndWait();
            var toss = await _session.Query<TossEntity>().FirstOrDefaultAsync();
            Assert.NotNull(toss);
            Assert.Equal("lorem ipsum lorem ipsum lorem ipsum lorem ipsum", toss.Content);
        }

        [Fact]
        public async Task create_when_display_count_creates_SponsoredToss()
        {
            await _mediator.Send(new TossCreateCommand()
            {
                Content = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum",
                SponsoredDisplayedCount = 1000
            });

            await SaveAndWait();

            var toss = await _session.Query<TossEntity>().FirstOrDefaultAsync();
            Assert.NotNull(toss);
            var sponsored = Assert.IsAssignableFrom<SponsoredTossEntity>(toss);
            Assert.Equal(1000, sponsored.DisplayedCount);
            Assert.Equal(1000, sponsored.DisplayedCountBought);
        }

        [Fact]
        public async Task create_when_sponsored_payment_succeed_return_ok()
        {
            await _mediator.Send(new TossCreateCommand()
            {
                Content = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum",
                SponsoredDisplayedCount = 1000,
                StripeChargeToken = "AAA"
            });

            var toss = await _session.Query<TossEntity>().FirstOrDefaultAsync();
            //_mockStripe.Verify(m => m.Charge("AAA", command.SponsoredDisplayedCount.Value * TossCreateCommand.CtsCostPerDisplay, It.Is<string>(s => s.Contains(toss.Id)), _m.ApplicationUser.Email));
        }

        [Fact]
        public async Task create_when_payment_fails_delete_toss_throw_error()
        {
            FakeStripeClient.NextCallFails = true;
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _mediator.Send(new TossCreateCommand()
                {
                    Content = "lorem ipsum erzer zer zer ze rze rezr zer",
                    SponsoredDisplayedCount = 1000,
                    StripeChargeToken = "AAA"
                }));
            var toss = await _session.Query<TossEntity>().FirstOrDefaultAsync();

            Assert.Null(toss);
        }

        [Fact]
        public async Task returns_sponsored_toss_from_sponsored_toss_query_and_dont_disply_after_count_done()
        {

            await _mediator.Send(new TossCreateCommand()
            {
                Content = "non sponsored toss content bla bla #toto"
            });
            await _mediator.Send(new TossCreateCommand()
            {
                Content = "lorem ipsum erzer zer zer ze rze rezr zer #toto",
                SponsoredDisplayedCount = 10,
                StripeChargeToken = "AAA"
            });
            await SaveAndWait();

            for (int i = 0; i < 10; i++)
            {
                var res = await _mediator.Send(new SponsoredTossQuery("toto"));

                Assert.NotNull(res);
                Assert.Equal("lorem ipsum erzer zer zer ze rze rezr zer #toto", res.Content);
            }
            await SaveAndWait();

            var resNull = await _mediator.Send(new SponsoredTossQuery("toto"));

            Assert.Null(resNull);


        }

        [Fact]
        public async Task sponsored_toss_displayed_randomly_among_users()
        {
            await _mediator.Send(new TossCreateCommand()
            {
                Content = "lorem ipsum erzer zer zer ze rze rezr zer from user 1 #toto",
                SponsoredDisplayedCount = 10,
                StripeChargeToken = "AAA"
            });
            await CreateNewUserIfNotExists("test2");

            await _mediator.Send(new TossCreateCommand()
            {
                Content = "lorem ipsum erzer zer zer ze rze rezr zer from user 2 #toto",
                SponsoredDisplayedCount = 10,
                StripeChargeToken = "AAA"
            });
            await SaveAndWait();

            RandomFake.NextResults.Enqueue(0);//first user selection
            RandomFake.NextResults.Enqueue(0);//first toss selection
            RandomFake.NextResults.Enqueue(1);//second user selection
            RandomFake.NextResults.Enqueue(0);//second toss selection

            var res = await _mediator.Send(new SponsoredTossQuery("toto"));
            Assert.NotNull(res);
            Assert.Contains("user 1", res.Content);

            res = await _mediator.Send(new SponsoredTossQuery("toto"));
            Assert.NotNull(res);
            Assert.Contains("user 2", res.Content);
        }

        [Fact]
        public async Task sponsored_toss_displayed_randomly_among_toss_of_one_user()
        {
            await _mediator.Send(new TossCreateCommand()
            {
                Content = "lorem ipsum erzer zer zer ze rze rezr zer from user 1 toss no 1  #toto",
                SponsoredDisplayedCount = 10,
                StripeChargeToken = "AAA"
            });
            await _mediator.Send(new TossCreateCommand()
            {
                Content = "lorem ipsum erzer zer zer ze rze rezr zer from user 1 toss no 2 #toto",
                SponsoredDisplayedCount = 10,
                StripeChargeToken = "AAA"
            });


            RandomFake.NextResults.Enqueue(0);//first user selection
            RandomFake.NextResults.Enqueue(0);//first toss selection
            RandomFake.NextResults.Enqueue(0);//second user selection
            RandomFake.NextResults.Enqueue(1);//second toss selection


            await SaveAndWait();

            var res = await _mediator.Send(new SponsoredTossQuery("toto"));
            Assert.NotNull(res);
            Assert.Contains("toss no 1", res.Content);


            res = await _mediator.Send(new SponsoredTossQuery("toto"));
            Assert.NotNull(res);
            Assert.Contains("toss no 2", res.Content);
        }

        [Fact]
        public async Task create_initalize_tags()
        {
            await _mediator.Send(new TossCreateCommand()
            {
                Content = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum #test #toto "
            });


            await SaveAndWait();
            var toss = await _session.Query<TossEntity>().FirstOrDefaultAsync();
            Assert.NotNull(toss.Tags);
            Assert.Equal(2, toss.Tags.Count);
            Assert.Contains("test", toss.Tags);
            Assert.Contains("toto", toss.Tags);
        }


        [Fact]
        public async Task create_initalize_tags_next_to_char()
        {
            await _mediator.Send(new TossCreateCommand()
            {
                Content = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum#test "
            });


            await SaveAndWait();
            var toss = await _mediator.Send(new TossLastQuery("test"));
            Assert.Single(toss);
        }

        [Fact]
        public async Task create_adds_request_ip()
        {
            await _mediator.Send(new TossCreateCommand()
            {
                Content = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum #test #toto "
            });


            await SaveAndWait();
            var toss = await _session.Query<TossEntity>().FirstOrDefaultAsync();

            Assert.Equal(serviceProviderInitializer.HttpContextMock.Object.Connection.RemoteIpAddress.ToString(), toss.UserIp);

        }
    }
}
