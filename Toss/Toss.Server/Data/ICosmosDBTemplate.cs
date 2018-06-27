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

        Task Insert(T instance);
    }
}