using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Toss.Server.Services;
using Toss.Shared;
using Toss.Shared.Tosses;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Account
{
    public class EditAccountCommandHandlerTest : BaseTest
    {
        [Fact]
        public async Task when_edit_user_name_change_user_name_on_previously_posted_toss()
        {
            await _mediator.Send(new TossCreateCommand("bla bla bla bla bla #test"));

            await _mediator.Send(new EditAccountCommand()
            {
                Email = "username@yopmil.com",
                Name = "tutu"
            });

            var res = await _mediator.Send(new TossLastQuery("test"));

            Assert.Equal("tutu", res.First().UserName);
        }

        [Fact]
        public async Task edit_email_send_email_with_confirmation_link()
        {
            await _mediator.Send(new EditAccountCommand()
            {
                Name = "username",
                Email = "toto@yopmail.com"
            });

            var emailSender = serviceProviderInitializer.GetInstance<IEmailSender>() as FakeEmailSender;
            Assert.NotNull(emailSender.GetConfirmationLink("toto@yopmail.com"));
        }

    }
}
