using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Toss.Shared;

namespace Toss.Tests.Server.Controllers
{
    [Authorize, Route("api/upload")]
    public class UploadController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Save()
        {
            var tempFileName = Path.GetTempFileName();
            using (var writer = System.IO.File.OpenWrite(tempFileName))
            {
                await Request.Body.CopyToAsync(writer);
            }
            return Ok(new FileUploadResult { TempFileName = Path.GetFileNameWithoutExtension(tempFileName) });
        }
    }
}
