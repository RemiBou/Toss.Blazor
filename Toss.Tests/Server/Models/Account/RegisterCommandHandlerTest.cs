using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toss.Server.Services;
using Toss.Shared.Account;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Account
{
    public class RegisterCommandHandlerTest : BaseTest
    {
        [Fact]
        public async Task cannot_register_if_captcha_fails()
        {
            var validator = (FakeCaptchaValidator)serviceProviderInitializer.GetInstance<ICaptchaValidator>();
            validator.NextResult = false;
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _mediator.Send(new RegisterCommand()
            {
                Password = "123456azerty!",
                Name = "remibou",
                Email = "remibou@yopmail.com"
            }));

        }

        [Fact]
        public async Task can_register_if_captcha_succeeds()
        {
            var validator = (FakeCaptchaValidator)serviceProviderInitializer.GetInstance<ICaptchaValidator>();
            validator.NextResult = true;
            var res = await _mediator.Send(new RegisterCommand()
            {
                Password = "123456Azerty!",
                Name = "remibou",
                Email = "remibou@yopmail.com"
            });
            Assert.True(res.IsSucess, res.Errors != null ? string.Join(",", res.Errors.SelectMany(e => e.Value)) : "");
        }

        [Fact]
        public async Task register_send_email_with_confirmation_link()
        {
            var res = await _mediator.Send(new RegisterCommand()
            {
                Password = "123456Azerty!",
                Name = "remibou",
                Email = "remibou@yopmail.com"
            });
            Assert.True(res.IsSucess, res.Errors != null ? string.Join(",", res.Errors.SelectMany(e => e.Value)) : "");
            var emailSender = serviceProviderInitializer.GetInstance<IEmailSender>() as FakeEmailSender;
            Assert.NotNull(emailSender.GetConfirmationLink("remibou@yopmail.com"));
        }

    }
}
