using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Toss.Server;
using Toss.Server.Controllers;
using Toss.Server.Data;
using Toss.Shared;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Controllers
{
    public class TossControllerTests
    {
        private readonly TossController _sut;
        private CommonMocks<TossController> _m = new CommonMocks<TossController>();
        public TossControllerTests()
        {

            _sut = new TossController(_m.Mediator.Object);
            _m.SetControllerContext(_sut);
        }

        [Fact]
        public async Task last_return_50_toss_from_mediator()
        {
            _m
                .Mediator
                .Setup(m => m.Send(It.Is<LastTossQuery>(q => q.HashTag == "toto"), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Range(0, 50)
                .Select(i => new TossLastQueryItem()));


            var res = await _sut.Last(new LastTossQuery("toto")) as OkObjectResult;
            var resValue = (res.Value as IEnumerable<TossLastQueryItem>);

            Assert.Equal(50, resValue.Count());
        }
        [Fact]
        public async Task last_map_toss_to_viewmodel()
        {
            _m
                 .Mediator
                 .Setup(m => m.Send(It.IsAny<LastTossQuery>(), It.IsAny<CancellationToken>()))
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
            Assert.IsAssignableFrom<IEnumerable<TossLastQueryItem>>(resOkObject.Value);

        }

        [Fact]
        public async Task create_calls_mediator_with_command()
        {
            var command = new TossCreateCommand()
            {
                Content = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum"
            };
            var res = await _sut.Create(command);

            _m.Mediator.Verify(r => r.Send(command,It.IsAny<CancellationToken>()));

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




        
    }
}
