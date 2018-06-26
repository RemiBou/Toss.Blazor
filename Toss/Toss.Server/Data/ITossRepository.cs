using System.Collections.Generic;
using System.Threading.Tasks;
using Toss.Shared;
using Toss.Shared.Tosses;

namespace Toss.Server.Data
{
    public interface ITossRepository
    {
        Task Create(TossCreateCommand oneToss);
        Task<IEnumerable<TossLastQueryItem>> Last(int count, string hashTag);
    }
}