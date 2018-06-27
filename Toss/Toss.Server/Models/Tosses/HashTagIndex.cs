using System.Collections.Generic;
using System.Text.RegularExpressions;
using Toss.Shared.Account;

namespace Toss.Server.Data
{
    /// <summary>
    /// Used for indexing tosses by its hashtags
    /// Partition Key will be the hashtag, Rowkey the toss's RowKey
    /// </summary>
    public class HashTagIndex 
    {
        private static readonly Regex regexHashTag = new Regex("(?<=#)"+AddHashtagCommand.HashTagRegex);
        public const string PartionKeyPrefix = "TossTag";
        public HashTagIndex(TossEntity toss,string hashTag)
        {
          
        }
        /// <summary>
        /// Creates all the hashtag indexes needed for a single toss
        /// </summary>
        /// <param name="toss"></param>
        /// <returns></returns>
        public static IEnumerable<HashTagIndex> CreateHashTagIndexes(TossEntity toss)
        {
            var matches = regexHashTag.Matches(toss.Content);
            foreach (Match m in matches)
            {
                yield return new HashTagIndex(toss, m.Value);
            }
        }
    }
}
