using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Raven.Client.Documents.Session;

namespace Toss.Server
{
    public class RavenDBSaveAsyncActionFilter : IAsyncActionFilter
    {
        private readonly IAsyncDocumentSession session;

        public RavenDBSaveAsyncActionFilter(IAsyncDocumentSession session)
        {
            this.session = session;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            // do something before the action executes
            var resultContext = await next();
            if (resultContext.Exception == null && !resultContext.Canceled)
            {
                await this.session.SaveChangesAsync();
            }
        }
    }
}