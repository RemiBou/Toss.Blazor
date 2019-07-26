using System.Threading.Tasks;

namespace Toss.Server.Services
{
    public interface IEmailSender
    {
        Task SendEmailConfirmationAsync(string email, string userName, string confirmationLink);
        Task SendNewConversation(string email, string conversationUserName, string tossUrl);
        Task SendPasswordForgetAsync(string email, string userName, string passwordResetLink);
    }
}
