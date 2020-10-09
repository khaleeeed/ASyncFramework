using ASyncFramework.Domain.Interface;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Infrastructure.Persistence.NestRepo.Configurations
{
    public abstract class GenericFaiulerRepository<T> : GenericRepository<T>,IGenericFaiulerRepository<T> where T : class
    {
        public GenericFaiulerRepository(IElasticClient client):base(client)
        {
        }
    
        public virtual async Task<(IEnumerable<T> doc, long total)> GetAllFaulierDocument(int from)
        {
            var search = await _Client.SearchAsync<T>(s =>
               s.Index($"{IndexName}*")
               .From(from)
               .Size(10)
                    .Query(q => q
                        .Bool(b => b
                          .Must(m => m
                           .Match(m => m
                                .Field(f => f.Suffix("fields.IsSendSuccessfully"))
                                    .Query("false")),
                                  m=>m
                                    .DateRange(m => m.GreaterThanOrEquals(DateMath.Anchored(DateTime.Now.AddMonths(-3)))))))
               .Sort(s => s
               .Ascending(a => a
               .Suffix("fields.CreationDate"))));

            return (search.Documents, search.Total);
        }
        public virtual async Task<(IEnumerable<T> doc, long total)> GetAllFaulierDocumentBySystemCode(int from , string system)
        {
            var search = await _Client.SearchAsync<T>(s =>
            s.Index($"{IndexName}*")
            .From(from)
            .Size(10)
                 .Query(q => q
                     .Bool(b => b
                       .Must(m => m
                        .Match(m => m
                             .Field(f => f.Suffix("fields.IsSendSuccessfully"))
                                 .Query("false")),
                             m => m.Match(f => f
                             .Field(f => f.Suffix("fields.System"))
                                 .Query(system)),
                               m => m
                                 .DateRange(m => m.GreaterThanOrEquals(DateMath.Anchored(DateTime.Now.AddMonths(-3)))))))
            .Sort(s => s
            .Ascending(a => a
            .Suffix("fields.CreationDate"))));

            return (search.Documents, search.Total);
        }
        public virtual async Task UpdateFaulier(string referenceNumber,bool isSuccessfully)
        {
            var update = await _Client.UpdateByQueryAsync<T>(s =>
             s.Index($"{IndexName}*")
             .Query(q=>q
                .Match(m=>m
                    .Field(f=>f.Suffix("fields.ReferenceNumber"))
                        .Query(referenceNumber)))
             .Script("ctx._source.fields['Retry']++;" +
                        $"ctx._source.fields['IsSendSuccessfully']={isSuccessfully.ToString().ToLower()};" +
                        $"ctx._source.fields['IsProcessing']=false")                                 
            );
            
        }
        public virtual async Task<bool> UpdateFaulierProcessing(string referenceNumber)
        {
            var update = await _Client.UpdateByQueryAsync<T>(s =>
             s.Index($"{IndexName}*")
             .Query(q => q
                .Match(m => m
                    .Field(f => f.Suffix("fields.ReferenceNumber"))
                        .Query(referenceNumber)))
             .Script("ctx._source.fields['Retry']++;" +                        
                        $"ctx._source.fields['IsProcessing']=true")
            );

            return update.Updated >= 1;
        }
        public virtual async Task<bool> UpdateFaulierProcessing(List<string> referenceNumber)
        {
            var update = await _Client.UpdateByQueryAsync<T>(s =>
             s.Index($"{IndexName}*")
             .Query(q => q
                .Terms(m => m
                    .Field(f => f.Suffix("fields.ReferenceNumber"))
                        .Terms(referenceNumber)))
             .Script("ctx._source.fields['Retry']++;" +
                        $"ctx._source.fields['IsProcessing']=true")
            );

            return update.Updated >= 1;
        }
        public override async Task<T> FindDocument(string referenceNumber)
        {
            var search = await _Client.SearchAsync<T>(s =>
                         s.Index($"{IndexName}*")
                         .From(0)
                         .Size(1)
                         .Query(q => q
                            .Bool(b => b
                                .Must(m => m
                                    .Match(m => m
                                        .Field(f => f.Suffix("fields.ReferenceNumber"))
                                            .Query(referenceNumber)),
                                     m => m.Match(f => f
                                        .Field(f => f.Suffix("fields.IsProcessing"))
                                            .Query("true"))
                               ))));

            return search.Documents?.FirstOrDefault();
        }
        public virtual async Task<bool> ReverseFaulierProcessing(string referenceNumber)
        {
            var update = await _Client.UpdateByQueryAsync<T>(s =>
             s.Index($"{IndexName}*")
             .Query(q => q
                .Match(m => m
                    .Field(f => f.Suffix("fields.ReferenceNumber"))
                        .Query(referenceNumber)))
             .Script("ctx._source.fields['Retry']--;" +
                        $"ctx._source.fields['IsProcessing']=false")
            );

            return update.Updated >= 1;
        }

    }
}