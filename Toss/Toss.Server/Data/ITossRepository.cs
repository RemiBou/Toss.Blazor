using System.Collections.Generic;
using System.Threading.Tasks;
using Toss.Shared;

namespace Toss.Server.Data
{
    public interface ITossRepository
    {
        Task Create(TossCreateCommand oneToss);
        Task<IEnumerable<TossLastQueryItem>> Last(int count, string hashTag);
    }
}