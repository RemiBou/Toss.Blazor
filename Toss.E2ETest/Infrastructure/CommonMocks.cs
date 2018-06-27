using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Toss.Server.Models;
using Toss.Server.Services;

namespace Toss.Tests.Infrastructure
{
    public class CommonMocks<TLogger>
    {
        public Mock<UserManager<ApplicationUser>> UserManager { get; set; }

        public Mock<SignInManager<ApplicationUser>> SignInManager { get; set; }

        public ClaimsPrincipal User { get; }

        public ApplicationUser ApplicationUser { get; }
        public Mock<IHttpContextAccessor> HttpContextAccessor { get; }
        public Mock<IEmailSender> EmailSender { get; }

        internal Mock<ILogger<TLogger>> Logger { get; }

        public Mock<IMediator> Mediator { get; set; }

        public CommonMocks()
        {
            Mediator = new Mock<IMediator>();
            HttpContextAccessor = new Mock<IHttpContextAccessor>();
            EmailSender = new Mock<IEmailSender>();
            UserManager = MockHelpers.MockUserManager<ApplicationUser>();
            SignInManager = MockHelpers.MockSigninManager(UserManager.Object);
            Logger = new Mock<ILogger<TLogger>>();
            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                     {
                            new Claim(ClaimTypes.Name, "username")
                     }, "someAuthTypeName"));
            ApplicationUser = new ApplicationUser() { UserName = "username", PasswordHash = "XXX" };

            UserManager.Setup(u => u.GetUserAsync(User))
                .ReturnsAsync(ApplicationUser);

            HttpContextAccessor
                .SetupGet(h => h.HttpContext)
                .Returns(new DefaultHttpContext() { User = User });
        }

        public void SetControllerContext(Controller controller)
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = User
                }
            };
        }

        public void SetControllerContext(ControllerBase controller)
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = User
                }
            };
        }
    }
}
