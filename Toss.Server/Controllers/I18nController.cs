using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Toss.Server
{
    public class Client { }//this class is just here for naming the ressource files
}
namespace Toss.Server.Controllers
{

    [ApiController, Route("api/[controller]/")]
    public class I18nController : ControllerBase
    {
        private IStringLocalizer<Client> stringLocalizer;

        public I18nController(IStringLocalizer<Client> stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
        }

        [HttpGet]
        public ActionResult GetClientTranslations()
        {
            var res = new Dictionary<string, string>();
            return Ok(stringLocalizer.GetAllStrings().ToDictionary(s => s.Name, s => s.Value));
        }
    }
}
