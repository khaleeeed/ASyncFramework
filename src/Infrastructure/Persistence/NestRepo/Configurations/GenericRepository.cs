using ASyncFramework.Domain.Interface;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Infrastructure.Persistence.NestRepo.Configurations
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T:class
    {
        public abstract string IndexName { get; }
        protected readonly IElasticClient _Client;
        public GenericRepository(IElasticClient client)
        {
            _Client = client;
        }

        public virtual async Task<bool> Add(T doc)
        {
            var response = await _Client.IndexAsync(doc, x => x.Index($"{IndexName}-{DateTime.Now:yyyy-MM}"));
            return response.IsValid;
        }

        public virtual async Task<T> FindDocument(string referenceNumber)
        {
            var search = await _Client.SearchAsync<T>(s =>
                         s.Index($"{IndexName}-*")
                         .From(0)
                         .Size(1)
                              .Query(q => q
                                     .Match(m => m
                                          .Field(f => f.Suffix("fields.ReferenceNumber")))));

            return search.Documents.FirstOrDefault();
        }

        public virtual async Task<(IEnumerable<T> doc, long total)> GetAllDocument(int from)
        {
            var search = await _Client.SearchAsync<T>(s =>
               s.Index($"{IndexName}-*")
               .From(from)
               .Size(10)                   
               .Sort(s => s
               .Ascending(a => a
               .Suffix("fields.CreationDate"))));

            return (search.Documents, search.Total);
        }

     

    }
}
