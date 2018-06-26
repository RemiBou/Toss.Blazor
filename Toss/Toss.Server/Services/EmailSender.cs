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

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            MailjetClient client = new MailjetClient(
                Configuration.GetValue<string>("MailJetApiKey"),
                 Configuration.GetValue<string>("MailJetApiSecret"));
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
               .Property(Send.FromEmail, Configuration.GetValue<string>("MailJetSender"))
               .Property(Send.FromName, "Toss")
               .Property(Send.Subject, subject)
               .Property(Send.HtmlPart, message)
               .Property(Send.Recipients, new JArray {
                new JObject {
                 {"Email", email}
                 }
                   });
            MailjetResponse response = await client.PostAsync(request);
        }

        public async Task SendEmailConfirmationAsync(string email, string userName, string confirmationLink)
        {
            var client = new MailjetClient(
                    Configuration.GetValue<string>("MailJetApiKey"),
                    Configuration.GetValue<string>("MailJetApiSecret"))
            {
                Version = ApiVersion.V3_1
            };
            //var request = new MailjetRequest{Resource = Send.Resource}
            //    .Property(Send.FromEmail, )
            //   .Property(Send.FromName, "Toss")
            //   .Property(
            //        Send.Messages, 
            //        new JArray {
            //            new JObject {
            //                {"To", new JArray {new JObject {{"Email", email},{"Name", userName}}}},
            //                {"TemplateID", 462653},
            //                {"TemplateLanguage", true},
            //                {"Subject", "Welcome to TOSS, please confirm your email"},
            //                {"Variables",
            //                    new JObject {
            //                        {"name", userName},
            //                        {"confirmation_link}}",confirmationLink}
            //                    }
            //                }
            //            }
            //        }
            //    );
            //var response = await client.PostAsync(request);
            //if (!response.IsSuccessStatusCode)
            //    throw new System.Exception(response.GetErrorMessage()+response.StatusCode+response.GetErrorInfo());
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
           .Property(Send.Messages, new JArray {
                new JObject {
                     {"From", new JObject {{"Email", Configuration.GetValue<string>("MailJetSender")},{"Name", "Toss"}}},
                     {"To", new JArray {new JObject {{"Email", email},{"Name", userName}}}},
                     {"TemplateID", 462653},
                     {"TemplateLanguage", true},
                     {"TemplateErrorDeliver", true},
                     //{"TemplateErrorReporting",new JObject  {{"Email", "" }, {"Name", "Rémi Bourgarel" } } },
                     {"Subject", "Welcome to TOSS, please confirm your email"},
                     {"Variables", new JObject 
                      {
                        {"name", userName},
                        {"confirmation_link", confirmationLink}
                      }
                     }
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
    }
}
