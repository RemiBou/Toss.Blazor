using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Toss.Shared;

namespace Toss.Server.Services
{

    public class CaptchaMediatRAdapter<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private ICaptchaValidator captchaValidator;

        public CaptchaMediatRAdapter(ICaptchaValidator captchaValidator)
        {
            this.captchaValidator = captchaValidator;
        }
        

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request is NotARobot notARobot)
            {
                await this.captchaValidator.Check(notARobot.Token);
            }
           
            return await next();
        }
    }
}
