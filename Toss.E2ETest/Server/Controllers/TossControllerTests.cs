using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Toss.Server;
using Toss.Server.Controllers;
using Toss.Server.Data;
using Toss.Shared;
using Xunit;

namespace Toss.Tests.Server.Controllers
{
    public class TossControllerTests
    {
        private TossController _sut;
        private Mock<ITossRepository> mockTossRepository;
        public TossControllerTests()
        {
            mockTossRepository = new Mock<ITossRepository>();
            _sut = new TossController(mockTossRepository.Object);
            _sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                     {
                            new Claim(ClaimTypes.Name, "username")
                     }, "someAuthTypeName"))
                }
            };
        }

        [Fact]
        public async Task last_return_50_toss_from_repo()
        {
            mockTossRepository
                .Setup(m => m.Last(50,"toto"))
                .ReturnsAsync(Enumerable.Range(0, 50)
                .Select(i => new TossLastQueryItem()));


            var res = await _sut.Last("toto") as OkObjectResult;
            var resValue = (res.Value as List<TossLastQueryItem>);

            Assert.Equal(50, resValue.Count());
        }
        [Fact]
        public async Task last_map_toss_to_viewmodel()
        {
            mockTossRepository
               .Setup(m => m.Last(50,null))
               .ReturnsAsync(Enumerable.Range(0, 1)
               .Select(i => new TossLastQueryItem()
               {
                   UserName = "toss@yopmail.com",
                   Content = "lorem ipsum",
                   CreatedOn = new System.DateTime(2018, 1, 1)                   
               })
               .ToList());

            var res = await _sut.Last(null) as OkObjectResult;
            var first = (res.Value as List<TossLastQueryItem>).First();
            var firstTyped = Assert.IsType<TossLastQueryItem>(first);
            Assert.Equal("toss@yopmail.com", firstTyped.UserName);
            Assert.Equal("lorem ipsum", firstTyped.Content);
            Assert.Equal(new System.DateTime(2018, 1, 1), firstTyped.CreatedOn);

        }

        [Fact]
        public async Task last_return_http_200()
        {
            var res = await _sut.Last(null);

            var resOkObject = Assert.IsType<OkObjectResult>(res);
            Assert.IsType<List<TossLastQueryItem>>(resOkObject.Value);

        }

        [Fact]
        public async Task create_calls_repo_with_command()
        {
            var command = new TossCreateCommand()
            {
                Content = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum"
            };
            var res = await _sut.Create(command);

            mockTossRepository.Verify(r => r.Create(command));

        }
        [Fact]
        public async Task create_return_ok_if_valid()
        {
            var command = new TossCreateCommand()
            {
                Content = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum"
            };
            var res = await _sut.Create(command);

            Assert.IsType<OkResult>(res);

        }


       

        [Fact]
        public async Task create_setup_username_to_current_user()
        {

            var command = new TossCreateCommand()
            {
                Content = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum",
                UserId="jeanmichel"
            };
            var res = await _sut.Create(command);

            mockTossRepository.Verify(r => r.Create(It.Is<TossCreateCommand>(c => c.UserId == "username")));

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
            var res = await _sut.Create(command);

            var now2 = DateTime.Now;
            mockTossRepository.Verify(r => r.Create(It.Is<TossCreateCommand>(c => c.CreatedOn >= now && c.CreatedOn <= now2)));

        }
    }
}
