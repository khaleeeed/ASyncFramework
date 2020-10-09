using ASyncFramework.Domain.Documents;
using ASyncFramework.Domain.Enums;
using ASyncFramework.Domain.Interface;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Infrastructure.Persistence.NestRepo
{
    public class ASyncFrameworkInfrastructureRepository: IASyncFrameworkInfrastructureRepository
    {
        public const string IndexName = "asyncframework-infrastructure-*";
        private readonly IElasticClient _Client;

        public ASyncFrameworkInfrastructureRepository(IElasticClient client)
        {
            _Client = client;
        }

        public async Task<object> GetMessageStatus(string referenceNumber,int from)
        {
            var search = await _Client.SearchAsync<AsyncframeworkInfrastructureDocument>(s =>
                s.Index(IndexName)
                .From(from)
                .Size(10)
                .Query(q => q
                    .Match(m => m
                    .Field(f => f.Fields.ReferenceNumber.Suffix("keyword"))
                    .Query(referenceNumber)))
                .Source(s => s.Excludes
                    (i => i.Field
                    (f => f.Fields.ResponseContent)
                    .Field(f => f.Fields.Url)
                    .Field(f=>f.Fields.CallBackUrl)
                    .Field(f=>f.Fields.TargetUrl)
                    .Field(f=>f.Fields.Hash)))
                .Sort(s=>s
                .Ascending(a=>a
                .Fields.CreationDate))
                 );

            return new { search.Total, Status = search.Documents};
        }
               
        public async Task<object> GetCallBackResponse(string referenceNumber, int from)
        {
            var search = await _Client.SearchAsync<AsyncframeworkInfrastructureDocument>(s =>
                s.Index(IndexName)
                .From(from)
                .Size(10)
                .Query(q => q
                    .Bool(b => b
                    .Must(m => m
                         .Match(m => m
                            .Field(f => f.Fields.ReferenceNumber.Suffix("keyword"))
                            .Query(referenceNumber)),
                         m=>m
                         .Match(m=>m
                         .Field(f=>f.Fields.Status.Suffix("keyword"))
                         .Query(MessageLifeCycle.SendRequestToCallBack.ToString())))))
                .Sort(s => s
                .Ascending(a => a
                .Fields.CreationDate))
                );
            
            return new { search.Total, Status = search.Documents };
        }
        
        public async Task<object> GetTargetResponse(string referenceNumber, int from)
        {
            var search = await _Client.SearchAsync<AsyncframeworkInfrastructureDocument>(s =>
                s.Index(IndexName)
                .From(from)
                .Size(10)
                .Query(q => q
                    .Bool(b => b
                        .Must(m => m
                            .Match(m => m
                                .Field(f => f.Fields.ReferenceNumber.Suffix("keyword"))
                                    .Query(referenceNumber)),
                            m => m
                                .Match(m => m
                         .Field(f => f.Fields.Status.Suffix("keyword"))
                         .Query(MessageLifeCycle.SendRequestToTarget.ToString())))))
                .Sort(s => s
                .Ascending(a => a
                .Fields.CreationDate))
                );
            
            return new { search.Total, Status = search.Documents };
        }
        
        public async Task<object> GetMessageFromCallBackUrl(string CallBackUrl, int from)
        {
            var search = await _Client.SearchAsync<AsyncframeworkInfrastructureDocument>(s =>
               s.Index(IndexName)
               .From(from)
               .Size(10)
               .Query(q => q
                   .Bool(b => b
                        .Must(m => m
                                .Match(m => m
                                    .Field(f => f.Fields.CallBackUrl.Suffix("keyword"))
                                        .Query(CallBackUrl)),
                              m => m
                                .Match(m => m
                                    .Field(f => f.Fields.Status.Suffix("keyword"))
                                        .Query(MessageLifeCycle.NewRequest.ToString())))))
               .Source(s => s.Excludes
                   (i => i.Field
                   (f => f.Fields.Hash)))
               .Sort(s => s
                   .Ascending(a => a
                    .Fields.CreationDate))
               );

            return new { search.Total, Status = search.Documents };
        }
        
        public (bool IsSendBefore, string ReferenceNumber) CheckIfRequestSendBefore(string hash)
        {
            var search = _Client.Search<AsyncframeworkInfrastructureDocument>(s =>
                           s.Index(IndexName)
                           .Size(1)
                           .Query(q => q
                               .Bool(b => b
                               .Must(m => m
                                    .Match(m => m
                                       .Field(f => f.Fields.Hash.Suffix("keyword"))
                                       .Query(hash)),
                                    m => m
                                    .Match(m => m
                                    .Field(f => f.Fields.Status.Suffix("keyword"))
                                    .Query(MessageLifeCycle.NewRequest.ToString())
                           )))));

            return (search.Documents.Count == 1,search.Documents.FirstOrDefault()?.Fields?.ReferenceNumber);
        }

    }   
}