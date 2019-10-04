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
        public readonly List<(string email, string userName, string link)> confirmationLinks = new List<(string email, string userName, string link)>();
        public readonly List<(string email, string userName, string link)> resetPasswordLinks = new List<(string email, string userName, string link)>();
        public readonly List<(string email, string tossCreatorUsername, string conversationUserName, string tossUrl)> newConversations = new List<(string email, string tossCreatorUsername, string conversationUserName, string tossUrl)>();

        public Task SendEmailConfirmationAsync(string email, string userName, string confirmationLink)
        {
            confirmationLinks.Add((email, userName, confirmationLink));
            return Task.CompletedTask;
        }

        public Task SendPasswordForgetAsync(string email, string userName, string passwordResetLink)
        {
            resetPasswordLinks.Add((email, userName, passwordResetLink));
            return Task.CompletedTask;
        }

        public Task SendNewConversationAsync(string email, string tossCreatorUserName, string conversationUserName, string tossUrl)
        {
            newConversations.Add((email, tossCreatorUserName, conversationUserName, tossUrl));
            return Task.CompletedTask;

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
