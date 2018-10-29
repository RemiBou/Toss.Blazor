using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toss.Server.Data;
using Toss.Server.Models;
using Xunit;

namespace Toss.Tests.Infrastructure
{  
    public class BaseCosmosTest : IAsyncLifetime
    {
        protected IMediator _mediator = TestFixture.GetInstance<IMediator>();
        protected UserManager<ApplicationUser> _userManager = TestFixture.GetInstance<UserManager<ApplicationUser>>();
        public BaseCosmosTest()
        {
        }

        public async Task InitializeAsync()
        {
           
            await TestFixture.CreateTestUser();
        }

        public async Task DisposeAsync()
        {
            var _client = TestFixture.GetInstance<DocumentClient>();
            var collections = _client.CreateDocumentCollectionQuery(UriFactory.CreateDatabaseUri(TestFixture.DataBaseName)).ToList();
            foreach (var item in collections)
            {
                var docs = _client.CreateDocumentQuery(item.SelfLink);
                foreach (var doc in docs)
                {
                    await _client.DeleteDocumentAsync(doc.SelfLink);
                }
            }
           // await Task.Delay(100);//mandatory for issuing queries after collection deletion
        }

        protected async Task EditCurrentUser(Action<ApplicationUser> toDo)
        {
            var user = await _userManager.GetUserAsync(TestFixture.ClaimPrincipal);
            toDo(user);
            await _userManager.UpdateAsync(user);
        }
    }
}
