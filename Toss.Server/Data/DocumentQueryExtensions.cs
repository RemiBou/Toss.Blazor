using System.Linq;

namespace Toss.Server.Data
{
    public static class DocumentQueryExtensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int page, int elementPerPage)
        {
            return query.Skip(elementPerPage * page)
                .Take(elementPerPage);

        }
    }
}