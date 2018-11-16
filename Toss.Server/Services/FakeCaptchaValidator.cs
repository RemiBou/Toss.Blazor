using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toss.Server.Services
{
    public class FakeCaptchaValidator : ICaptchaValidator
    {
        public bool NextResult { get; set; } = true;
        public Task Check(string token)
        {
            if (!NextResult)
                throw new InvalidOperationException();
            return Task.CompletedTask;
        }
    }
}
