using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Toss.Server.Controllers;
using Toss.Server.Data;
using Toss.Shared;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Tosses
{
    public class CreateTossCommandHandlerTest
    {
        private TossCreateCommandHandler _sut;
        private CommonMocks<TossController> _m;
        private Mock<ITossRepository> _repository;
        public CreateTossCommandHandlerTest()
        {
            _m = new CommonMocks<TossController>();
            _repository = new Mock<ITossRepository>();
            _sut = new TossCreateCommandHandler(
                _repository.Object,
                _m.HttpContextAccessor.Object);
        }
        [Fact]
        public async Task create_setup_username_to_current_user()
        {

            var command = new TossCreateCommand()
            {
                Content = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum",
                UserId = "jeanmichel"
            };
            var res = await _sut.Handle(command,new System.Threading.CancellationToken());

            _repository.Verify(r => r.Create(It.Is<TossCreateCommand>(c => c.UserId == "username")));

        }
        [Fact]
        public async Task create_setup_date_of_post_to_today()
        {

            var command = new TossCreateCommand()
            {
                Content = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum",
                UserId = "jeanmichel"
            };
            var now = DateTime.Now;
            var res = await _sut.Handle(command,new System.Threading.CancellationToken());

            var now2 = DateTime.Now;
            _repository.Verify(r => r.Create(It.Is<TossCreateCommand>(c => c.CreatedOn >= now && c.CreatedOn <= now2)));

        }
    }
}
