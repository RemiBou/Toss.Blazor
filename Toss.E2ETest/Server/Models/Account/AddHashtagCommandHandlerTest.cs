using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Toss.Server.Controllers;
using Toss.Server.Models;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Account
{
    public class AddHashtagCommandHandlerTest
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManager;
        private readonly ClaimsPrincipal _user;
        private readonly ApplicationUser _applicationUser;
        private readonly AddHashtagCommandHandler _sut;
        private readonly Mock<IHttpContextAccessor> _httpCOntextAccessor;

        public AddHashtagCommandHandlerTest()
        {
            _httpCOntextAccessor = new Mock<IHttpContextAccessor>();
            _userManager = MockHelpers.MockUserManager<ApplicationUser>();
            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                            new Claim(ClaimTypes.Name, "username")
                    }, "someAuthTypeName"));
            _applicationUser = new ApplicationUser() { UserName = "username", PasswordHash = "XXX" };
            _userManager.Setup(u => u.GetUserAsync(_user))
                        .ReturnsAsync(_applicationUser);

            _httpCOntextAccessor.SetupGet(h => h.HttpContext)
                                .Returns(new DefaultHttpContext() { User = _user });
            _sut = new AddHashtagCommandHandler(_userManager.Object, _httpCOntextAccessor.Object);
        }
        [Fact]
        public async Task AddHashTag_if_null_hashtag_returns_error()
        {
            var res = await _sut.Handle(new AddHashtagCommand(null), new System.Threading.CancellationToken());

            Assert.False(res.IsSucess);
            Assert.True(res.Errors.ContainsKey("newTag"));
            _userManager.Verify(u => u.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never);
        }
        [Fact]
        public async Task AddHashTag_calls_user_store_with_new_hashtag()
        {


            var res = await _sut.Handle(new AddHashtagCommand("toto"), new System.Threading.CancellationToken());

            _userManager.Verify(u => u.UpdateAsync(It.Is<ApplicationUser>(a => a.Hashtags.Contains("toto"))));
            Assert.True(res.IsSucess);
        }

        [Fact]
        public async Task AddHashTag_cannot_add_twice_same_hashtag()
        {


            var res = await _sut.Handle(new AddHashtagCommand("toto"), new System.Threading.CancellationToken());
            res = await _sut.Handle(new AddHashtagCommand("toto"), new System.Threading.CancellationToken());

            _userManager.Verify(u => u.UpdateAsync(It.Is<ApplicationUser>(a => a.Hashtags.Count == 1)), Times.Once);
            _userManager.Verify(u => u.UpdateAsync(It.Is<ApplicationUser>(a => a.Hashtags.Count == 2)), Times.Never);
            Assert.False(res.IsSucess);
            Assert.Contains("newTag",res.Errors.Keys);

        }

    }
}
