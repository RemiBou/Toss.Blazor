using Microsoft.Azure.Documents;
using System.Linq;
using System.Threading.Tasks;

namespace Toss.Server.Data
{
    public interface ICosmosDBTemplate<T>
    {
        Task<Database> GetDatabase();

        Task<DocumentCollection> GetCollection();

        Task<IOrderedQueryable<T>> CreateDocumentQuery();

        Task<string> Insert(T instance);
        Task Update(T instance);
        Task Delete(string id);
        Task<IQueryable<T>> CreateDocumentQuery(string sql);

        Task<IQueryable<TReturn>> CreateDocumentQuery<TReturn>(string sql);
        Task<IQueryable<TReturn>> CreateDocumentQuery<TReturn>();
    }
}