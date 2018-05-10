using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Toss.Server.Data
{
    /// <summary>
    /// Used for indexing tosses by its hashtags
    /// Partition Key will be the hashtag, Rowkey the toss's RowKey
    /// </summary>
    public class HashTagIndex : TableEntity
    {
        private static Regex regexHashTag = new Regex(@"(?<=#)\w+");
        public const string PartionKeyPrefix = "TossTag";
        public HashTagIndex(OneTossEntity toss,string hashTag)
        {
            PartitionKey = PartionKeyPrefix + hashTag;
            RowKey = toss.RowKey;
        }
        /// <summary>
        /// Creates all the hashtag indexes needed for a single toss
        /// </summary>
        /// <param name="toss"></param>
        /// <returns></returns>
        public static IEnumerable<HashTagIndex> CreateHashTagIndexes(OneTossEntity toss)
        {
            var matches = regexHashTag.Matches(toss.Content);
            foreach (Match m in matches)
            {
                yield return new HashTagIndex(toss, m.Value);
            }
        }
    }
}
