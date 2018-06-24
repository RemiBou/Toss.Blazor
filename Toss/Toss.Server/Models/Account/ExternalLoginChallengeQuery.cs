using MediatR;
using Microsoft.AspNetCore.Authentication;

namespace Toss.Server.Controllers
{
    public class ExternalLoginChallengeQuery : IRequest<AuthenticationProperties>
    {
        public ExternalLoginChallengeQuery(string providerName, string returnUrl)
        {
            if (string.IsNullOrEmpty(providerName))
            {
                throw new System.ArgumentException("provider mandatory", nameof(providerName));
            }



            ProviderName = providerName;
            ReturnUrl = returnUrl;
        }

        public string ProviderName { get; set; }
        public string ReturnUrl { get; set; }
    }
}
