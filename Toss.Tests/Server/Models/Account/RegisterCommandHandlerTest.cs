using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Toss.Server.Services;
using Toss.Shared.Account;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Account
{
    public class RegisterCommandHandlerTest : BaseCosmosTest
    {
        [Fact]
        public async Task CannotRegisterIfCaptachaValidationFails()
        {
            var validator =(FakeCaptchaValidator) TestFixture.GetInstance<ICaptchaValidator>();
            validator.NextResult = false;
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _mediator.Send(new RegisterCommand()
            {
                Password = "123456azerty!",
                Name = "remibou",
                Email = "remibou@yopmail.com"
            }));

        }

        [Fact]
        public async Task CanRegisterIfCaptachaValidationSucceeds()
        {
            var validator = (FakeCaptchaValidator) TestFixture.GetInstance<ICaptchaValidator>();
            validator.NextResult = true;
             await _mediator.Send(new RegisterCommand()
             {
                 Password="123456azerty!",
                 Name="remibou",
                 Email="remibou@yopmail.com"
             });

        }
    }
}
