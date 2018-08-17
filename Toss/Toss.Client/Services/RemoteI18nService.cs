using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Toss.Client.Services
{
    public class RemoteI18nService : II18nService
    {
        private readonly IHttpApiClientRequestBuilderFactory httpApiClientRequestBuilderFactory;
        private Lazy<Task< Dictionary<string, string>>> translations;

        public RemoteI18nService(IHttpApiClientRequestBuilderFactory httpApiClientRequestBuilderFactory)
        {
            this.httpApiClientRequestBuilderFactory = httpApiClientRequestBuilderFactory;
            translations = new Lazy<Task<Dictionary<string, string>>>(() => FetchTranslations(null));
        }

        private async Task<Dictionary<string, string>> FetchTranslations(string lg)
        {
            var client = httpApiClientRequestBuilderFactory.Create("/api/i18n");
            if (lg != null)
                client.SetHeader("accept-language", lg);
            
            Dictionary<string, string> res = null;
            await client.OnOK<Dictionary<string, string>>(r => res = r).Get();
            return res;
        }

        public async Task<string> Get(string key)
        {
            return !(await translations.Value).TryGetValue(key, out string value) ? key : value;
        }
       
        public void Init(string lg)
        {
            translations = new Lazy<Task<Dictionary<string, string>>>(() => FetchTranslations(lg));
        }
    }
}
