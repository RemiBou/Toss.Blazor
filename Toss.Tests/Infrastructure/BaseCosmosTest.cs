using MediatR;
using Microsoft.AspNetCore.Identity;
using Raven.Client.Documents;
using Raven.TestDriver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toss.Server.Data;
using Toss.Server.Models;
using Xunit;

namespace Toss.Tests.Infrastructure
{
    public class BaseTest : IAsyncLifetime
    {

        protected IMediator _mediator = TestFixture.GetInstance<IMediator>();
        protected UserManager<ApplicationUser> _userManager = TestFixture.GetInstance<UserManager<ApplicationUser>>();
        public BaseTest()
        {

        }



        protected async Task EditCurrentUser(Action<ApplicationUser> toDo)
        {
            var user = await _userManager.GetUserAsync(TestFixture.ClaimPrincipal);
            toDo(user);
            await _userManager.UpdateAsync(user);
            TestFixture.TestDriver.WaitIndexing();
        }

        public async Task InitializeAsync()
        {
            await TestFixture.CreateTestUser();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
