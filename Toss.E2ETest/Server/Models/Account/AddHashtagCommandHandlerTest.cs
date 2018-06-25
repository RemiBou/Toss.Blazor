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
using Toss.Server.Models.Account;
using Toss.Shared.Account;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Account
{
    public class AddHashtagCommandHandlerTest
    {
      
        private readonly AddHashtagCommandHandler _sut;
        private CommonMocks<AccountController> _m = new CommonMocks<AccountController>();

        public AddHashtagCommandHandlerTest()
        {
           
            _sut = new AddHashtagCommandHandler(_m.UserManager.Object, _m.HttpContextAccessor.Object);
        }
        [Fact]
        public async Task AddHashTag_if_null_hashtag_returns_error()
        {
            var res = await _sut.Handle(new AddHashtagCommand(null), new System.Threading.CancellationToken());

            Assert.False(res.IsSucess);
            Assert.True(res.Errors.ContainsKey("newTag"));
            _m.UserManager.Verify(u => u.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never);
        }
        [Fact]
        public async Task AddHashTag_calls_user_store_with_new_hashtag()
        {


            var res = await _sut.Handle(new AddHashtagCommand("toto"), new System.Threading.CancellationToken());

            _m.UserManager.Verify(u => u.UpdateAsync(It.Is<ApplicationUser>(a => a.Hashtags.Contains("toto"))));
            Assert.True(res.IsSucess);
        }

        [Fact]
        public async Task AddHashTag_cannot_add_twice_same_hashtag()
        {


            var res = await _sut.Handle(new AddHashtagCommand("toto"), new System.Threading.CancellationToken());
            res = await _sut.Handle(new AddHashtagCommand("toto"), new System.Threading.CancellationToken());

            _m.UserManager.Verify(u => u.UpdateAsync(It.Is<ApplicationUser>(a => a.Hashtags.Count == 1)), Times.Once);
            _m.UserManager.Verify(u => u.UpdateAsync(It.Is<ApplicationUser>(a => a.Hashtags.Count == 2)), Times.Never);
            Assert.False(res.IsSucess);
            Assert.Contains("newTag",res.Errors.Keys);

        }

    }
}
