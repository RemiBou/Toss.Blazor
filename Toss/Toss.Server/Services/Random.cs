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
    /// Fake implementation of randomwill return 0 every time unless NextResultis set
    /// </summary>
    public class RandomFake : IRandom
    {
        /// <summary>
        /// If changed will be set to 1 after each call to NewRandom
        /// </summary>
        public static int NextResult { get; set; } = 0;

        public int NewRandom(int maxValue)
        {
            var res = NextResult;
            NextResult = 0;
            return res;
        }
    }
}
