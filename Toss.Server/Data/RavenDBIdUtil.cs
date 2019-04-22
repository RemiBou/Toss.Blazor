using Raven.Client.Documents;

namespace Toss.Server.Data
{
    /// <summary>
    /// Used for changing an id from raven db to an id usable into a route (TossEntity/4654 becomes 4654)
    /// </summary>
    public  class RavenDBIdUtil
    {
        private IDocumentStore documentStore;
        
        public RavenDBIdUtil(IDocumentStore documentStore)
        {
            this.documentStore = documentStore;
        }

        public  string GetUrlId(string ravenDbId)
        {
            return ravenDbId.Split(documentStore.Conventions.IdentityPartsSeparator)[1];
        }

        public string GetRavenDbIdFromUrlId<T>(string urlId)
        {
            return $"{documentStore.Conventions.FindCollectionName(typeof(T))}{documentStore.Conventions.IdentityPartsSeparator}{urlId}";
        }
    }
}
