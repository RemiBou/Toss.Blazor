using System.Threading.Tasks;

namespace Toss.Server.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendEmailConfirmationAsync(string email, string userName, string confirmationLink);
    }
}
