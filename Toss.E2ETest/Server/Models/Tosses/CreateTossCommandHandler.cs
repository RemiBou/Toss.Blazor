using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toss.Server.Controllers;
using Toss.Server.Data;
using Toss.Shared;
using Toss.Shared.Tosses;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Tosses
{
    public class CreateTossCommandHandlerTest : BaseCosmosTest, IClassFixture<CosmosDBFixture>
    {
        private CommonMocks<TossController> _m;
        private ICosmosDBTemplate<TossEntity> tossTemplate;
        private TossCreateCommandHandler _sut;
        public CreateTossCommandHandlerTest(CosmosDBFixture fixture):base(fixture)
        {
            _m = new CommonMocks<TossController>();

            tossTemplate = GetTemplate<TossEntity>();
            _sut = new TossCreateCommandHandler(
                tossTemplate,
                _m.HttpContextAccessor.Object);
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
            var now = DateTime.Now;
            var res = await _sut.Handle(command, new System.Threading.CancellationToken());

            var now2 = DateTime.Now;

            var toss = await (await tossTemplate.CreateDocumentQuery()).GetFirstOrDefault();

            Assert.True(toss.CreatedOn >= now && toss.CreatedOn <= now2); ;

        }

        [Fact]
        public async Task create_insert_item_in_azure_table()
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


    }
}
