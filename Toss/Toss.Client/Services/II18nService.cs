using System.Collections.Generic;
using System.Threading.Tasks;

namespace Toss.Client.Services
{
    public interface II18nService
    {
        Task<string> Get(string name);

        void Init(string lg);
    }
}
