using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Toss.Server.Data;
using Toss.Server.Extensions;
using Toss.Shared;

namespace Toss.Server.Controllers
{
    [Authorize, ApiController, Route("api/[controller]/[action]")]
    public class TossController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TossController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Returns the last created Toss for the homepage
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Last(string hashTag)
        {
            return Ok(await _mediator.Send(new LastTossQuery() { HashTag = hashTag }));
        }

        /// <summary>
        /// Creates a new toss
        /// </summary>
        /// <param name="createTossCommand"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(TossCreateCommand createTossCommand)
        {
            await _mediator.Send(createTossCommand);
            return Ok();
        }
    }
}
