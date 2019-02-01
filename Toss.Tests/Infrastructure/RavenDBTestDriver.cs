using System;
using Raven.Client.Documents;
using Raven.TestDriver;

namespace Toss.Tests.Infrastructure
{
    public class RavenDBTestDriver : RavenTestDriver
    {
        //This allows us to modify the conventions of the store we get from 'GetDocumentStore'
        protected override void PreInitialize(IDocumentStore documentStore)
        {
            Console.WriteLine("Pre initialize");
            documentStore.Conventions.MaxNumberOfRequestsPerSession = 200;
        }

        public void ConfigureService()
        {
            ConfigureServer(
                 new TestServerOptions()
                 {
                     DataDirectory = "./RavenDBTest"
                 }
             );
        }
        public IDocumentStore GetDocumentStore()
        {
            return base.GetDocumentStore();
        }

    }
}