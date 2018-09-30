using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toss.Server.Services
{
    /// <summary>
    /// Necessary abstraction layer for autmated tests
    /// </summary>
    public interface IRandom
    {
        int NewRandom(int maxValue);

    }

    /// <summary>
    /// Implementation with System.Random without Seed, if needed I will add one
    /// </summary>
    public class RandomTrue : IRandom
    {
        private Random random = new Random();
        public int NewRandom(int maxValue)
        {
            return random.Next(maxValue);
        }
    }


    /// <summary>
    /// Fake implementation of randomwill return 0 every time unless NextResults set
    /// </summary>
    public class RandomFake : IRandom
    {
        /// <summary>
        /// Next result to return, if none, 0 willbe returned
        /// </summary>
        public static Queue<int> NextResults { get; set; } = new Queue<int>();

        public int NewRandom(int maxValue)
        {
            int res;
            if (NextResults.TryDequeue(out res))
                return res;
            return 0;
        }
    }
}
