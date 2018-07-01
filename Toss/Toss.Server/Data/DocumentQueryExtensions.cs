using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Linq;

namespace Toss.Server.Data
{
    public static class DocumentQueryExtensions
    {
        public async static Task<T[]> GetAllResultsAsync<T>(this IDocumentQuery<T> queryAll)
        {
            var list = new List<T>();

            while (queryAll.HasMoreResults)
            {
                var docs = await queryAll.ExecuteNextAsync<T>();
                
                foreach (var d in docs)
                {
                    list.Add(d);
                }
            }

            return list.ToArray();
        }
        public static async Task<T> GetFirstOrDefault<T>(this IOrderedQueryable<T> queryAll)
        {
            return (await queryAll.AsDocumentQuery().GetAllResultsAsync()).FirstOrDefault();
        }

        public static async Task<T> GetFirstOrDefault<T>(this IDocumentQuery<T> queryAll)
        {
            return (await queryAll.GetAllResultsAsync()).FirstOrDefault();
        }
    }
}
