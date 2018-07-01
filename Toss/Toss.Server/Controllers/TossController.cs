using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Toss.Shared.Tosses;

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
        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<IActionResult> List([FromQuery] TossListAdminQuery query)
        {
            return Ok(await _mediator.Send(query));
        }
        /// <summary>
        /// Returns the last created Toss for the homepage
        /// </summary>
        /// <returns></returns>
        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromBody] DeleteTossCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
        /// <summary>
        /// Returns the last created Toss for the homepage
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Last([FromQuery] TossLastQuery hashTag)
        {
            return Ok(await _mediator.Send(hashTag));
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
