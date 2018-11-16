using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toss.Shared;

namespace Toss.Server.Services
{

    public class CaptchaMediatRAdapter<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : NotARobot, IRequest<TResponse>
    {
        private ICaptchaValidator captchaValidator;

        public CaptchaMediatRAdapter(ICaptchaValidator captchaValidator)
        {
            this.captchaValidator = captchaValidator;
        }
        

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            await this.captchaValidator.Check(((NotARobot)request).Token);
            return await next();
        }
    }
}
