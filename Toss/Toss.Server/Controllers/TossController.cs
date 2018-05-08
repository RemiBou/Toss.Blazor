using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toss.Server.Data;
using Toss.Server.Extensions;
using Toss.Shared;

namespace Toss.Server.Controllers
{
    [Authorize]
    public class TossController : Controller
    {
        private readonly ITossRepository tossRepository;

        public TossController(ITossRepository tossRepository)
        {
            this.tossRepository = tossRepository;
        }
        /// <summary>
        /// Returns the last created Toss for the homepage
        /// </summary>
        /// <returns></returns>
        [HttpGet]       
        public async Task<IActionResult> Last()
        {
            return Ok((await tossRepository.Last(50)).ToList());
        }

        /// <summary>
        /// Creates a new toss
        /// </summary>
        /// <param name="createTossCommand"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(TossCreateCommand createTossCommand)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToFlatDictionary());
            createTossCommand.UserId = User.Identity.Name;
            createTossCommand.CreatedOn = DateTime.Now;
            await tossRepository.Create(createTossCommand);
            return Ok();
        }
    }
}
