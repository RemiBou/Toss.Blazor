using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toss.Server.Services
{
    /// <summary>
    /// Here for E2E tests, didn't find better place to put it
    /// </summary>
    public class FakeEmailSender : IEmailSender
    {
        private readonly List<Tuple<string, string, string>> confirationLinks = new List<Tuple<string, string, string>>();
        private readonly List<Tuple<string, string, string>> resetPasswordLinks = new List<Tuple<string, string, string>>();

        public Task SendEmailConfirmationAsync(string email, string userName, string confirmationLink)
        {
            confirationLinks.Add(new Tuple<string, string, string>(email, userName, confirmationLink));
            return Task.CompletedTask;
        }

        public Task SendPasswordForgetAsync(string email, string userName, string passwordResetLink)
        {
            resetPasswordLinks.Add(new Tuple<string, string, string>(email, userName, passwordResetLink));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Returns the last confirmation link found for given email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public string GetConfirmationLink(string email)
        {
            return confirationLinks.LastOrDefault(t => t.Item1 == email)?.Item3;
        }
    }

    public class FakeStripeClient : IStripeClient
    {
        public static bool NextCallFails { get; set; }

        public Task<bool> Charge(string token, int amount, string description, string email)
        {
            if (NextCallFails)
            {
                NextCallFails = false;
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }
    }


}
