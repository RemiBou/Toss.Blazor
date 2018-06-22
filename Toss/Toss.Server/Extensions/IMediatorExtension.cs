using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Toss.Shared.Account;

namespace Toss.Server.Extensions
{
    public static class IMediatorExtension
    {
        public static async Task<IActionResult> ExecuteCommandReturnActionResult(this IMediator mediator, IRequest<CommandResult> model)
        {
            var res = await mediator.Send(model);
            if (res.IsSucess)
                return new OkResult();
            return new BadRequestObjectResult(res.Errors);
        }

        public static async Task<IActionResult> ExecuteCommandReturnActionResult(this IMediator mediator, IRequest model)
        {
            var res = await mediator.Send(model);

            return new OkResult();
        }
    }
}
