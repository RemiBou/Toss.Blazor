using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Toss.Shared.Tosses;

namespace Toss.Server.Data
{
    /// <summary>
    /// Store and retreive Toss from an Azure Table Storage
    /// </summary>
    public class CosmosDBTemplate<T> : ICosmosDBTemplate<T>
    {
      
        private string collectionId;
        public CosmosDBTemplate(DocumentClient documentClient, IOptions<CosmosDBTemplateOptions> options) : this(documentClient, options.Value)
        {
        }
        public CosmosDBTemplate(DocumentClient documentClient, CosmosDBTemplateOptions options)
        {
            collectionId = typeof(T).GetType().Name;
            _documentClient = documentClient;
            
            _database = new Lazy<Task<ResourceResponse<Database>>>(() =>
                _documentClient.CreateDatabaseIfNotExistsAsync(new Database() { Id = options.DataBaseName })
            );
            _collection = new Lazy<Task<ResourceResponse<DocumentCollection>>>(async () =>
                {
                    DocumentCollection documentCollection = new DocumentCollection() { Id = collectionId };
                    documentCollection.IndexingPolicy = new IndexingPolicy(new RangeIndex(DataType.String) { Precision = -1 });
                    documentCollection.IndexingPolicy.IndexingMode = IndexingMode.Consistent;
                    return await _documentClient.CreateDocumentCollectionIfNotExistsAsync(
                        (await GetDatabase()).SelfLink,
                        documentCollection
                    );
                }
            );
        }
        private readonly DocumentClient _documentClient;
        private readonly Lazy<Task<ResourceResponse<Database>>> _database;
        private readonly Lazy<Task<ResourceResponse<DocumentCollection>>> _collection;

        public async Task<Database> GetDatabase()
        {
            return (await _database.Value).Resource;
        }

        public async Task<DocumentCollection> GetCollection()
        {
            return (await _collection.Value).Resource;
        }

        public async Task<IOrderedQueryable<T>> CreateDocumentQuery()
        {
            return _documentClient.CreateDocumentQuery<T>(await GetCollectionUri());
        }

        public async Task<IQueryable<T>> CreateDocumentQuery(string sql)
        {
            return _documentClient.CreateDocumentQuery<T>(await GetCollectionUri(), sql);
        }


        public async Task<IQueryable<TReturn>> CreateDocumentQuery<TReturn>()
        {
            return _documentClient.CreateDocumentQuery<TReturn>(await GetCollectionUri());
        }
        public async Task<IQueryable<TReturn>> CreateDocumentQuery<TReturn>(string sql)
        {
            return _documentClient.CreateDocumentQuery<TReturn>(await GetCollectionUri(), sql);
        }
        public async Task<string> Insert(T instance)
        {
            return (await _documentClient.CreateDocumentAsync(await GetCollectionUri(), instance)).Resource.Id;
        }

        public async Task Update(T instance)
        {
            var res = (await _documentClient.UpsertDocumentAsync(await GetCollectionUri(), instance)).Resource.Id;
        }

        private async Task<Uri> GetCollectionUri()
        {
            return UriFactory.CreateDocumentCollectionUri((await GetDatabase()).Id, (await GetCollection()).Id);
        }

        public async Task Delete(string id)
        {
            await _documentClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri((await GetDatabase()).Id, (await GetCollection()).Id, id));
        }
    }
}
