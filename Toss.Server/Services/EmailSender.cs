using System;
using System.Threading.Tasks;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Toss.Server.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration Configuration;

        public EmailSender(IConfiguration configuration)
        {
            Configuration = configuration;
        }



        public async Task SendPasswordForgetAsync(string email, string userName, string passwordResetLink)
        {
            await SendMailjetTemplate(email,userName, 462997, "TOSS account password reset", new JObject { { "name", userName }, { "confirmation_link", passwordResetLink } });
        }

        private async Task SendMailjetTemplate(string email,string userName, int templateId, string subject, JObject templateVariables)
        {
            var client = new MailjetClient(
                    Configuration.GetValue<string>("MailJetApiKey"),
                    Configuration.GetValue<string>("MailJetApiSecret"))
            {
                Version = ApiVersion.V3_1
            };
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
           .Property(Send.Messages, new JArray {
                new JObject {
                     {"From", new JObject {{"Email", Configuration.GetValue<string>("MailJetSender")},{"Name", "Toss"}}},
                     {"To", new JArray {new JObject {{"Email", email},{"Name", userName}}}},
                     {"TemplateID", templateId},
                     {"TemplateLanguage", true},
                     {"TemplateErrorDeliver", true},
                     //{"TemplateErrorReporting",new JObject  {{"Email", "" }, {"Name", "Rémi Bourgarel" } } },
                     {"Subject", subject},
                     {"Variables", templateVariables}
                 }
               }
            );
            MailjetResponse response = await client.PostAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    string.Format("StatusCode: {0}    ", response.StatusCode) +
                    string.Format("ErrorInfo: {0}    ", response.GetErrorInfo()) +
                    string.Format("GetData: {0}    ", response.GetData()) +
                    string.Format("ErrorMessage: {0}    ", response.GetErrorMessage()));
            }
        }

        public async Task SendEmailConfirmationAsync(string email, string userName, string confirmationLink)
        {
            await SendMailjetTemplate(email,userName, 462653, "Welcome to TOSS, please confirm your email", new JObject { { "name", userName }, { "confirmation_link", confirmationLink } });
        }
    }
}
