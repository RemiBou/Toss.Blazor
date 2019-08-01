﻿using System.Threading.Tasks;

namespace Toss.Server.Services
{
    public interface IEmailSender
    {
        Task SendEmailConfirmationAsync(string email, string userName, string confirmationLink);
        Task SendNewConversationAsync(string email, string tossCreatorUserName, string conversationUserName, string tossUrl);
        Task SendPasswordForgetAsync(string email, string userName, string passwordResetLink);
    }
}
