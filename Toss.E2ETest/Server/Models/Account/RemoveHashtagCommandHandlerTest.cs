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
    public class RemoveHashtagCommandHandlerTest
    {
      
        private readonly RemoveHashTagCommandHandler _sut;
        private CommonMocks<AccountController> _m = new CommonMocks<AccountController>();

        public RemoveHashtagCommandHandlerTest()
        {
           
            _sut = new RemoveHashTagCommandHandler(_m.UserManager.Object, _m.HttpContextAccessor.Object);
        }
       
        [Fact]
        public async Task RemoveHashTag_calls_user_store_with_hashtag_removed()
        {
            _m.ApplicationUser.AddHashTag("test");

            var res = await _sut.Handle(new RemoveHashTagCommand("test"), new System.Threading.CancellationToken());

            _m.UserManager.Verify(u => u.UpdateAsync(It.Is<ApplicationUser>(a => !a.Hashtags.Contains("test"))));
            Assert.True(res.IsSucess);
        }
        

    }
}
