using ASyncFramework.Domain.Documents;
using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Infrastructure.Persistence.NestRepo.Configurations;
using Nest;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASyncFramework.Infrastructure.Persistence.NestRepo
{
    public class LUTNotificationRepository : GenericRepository<NotificationDocument> , ILUTNotificationRepository
    {
        public override string IndexName => $"lutnotification";
        public LUTNotificationRepository(IElasticClient client):base(client)
        {

        }

        public async Task<(IEnumerable<NotificationDocument> doc, long total)> GetDocumentBySystemCode(int from, string systemCode)
        {
            var search = await _Client.SearchAsync<NotificationDocument>(s =>
            s.Index($"{IndexName}-*")
            .From(from)
            .Size(10)
                 .Query(q => q
                     .Match(f => f
                             .Field(f => f.Suffix("SystemCode"))
                                 .Query(systemCode)))
            .Sort(s => s
            .Ascending(a => a
            .Suffix("fields.CreationDate"))));

            return (search.Documents, search.Total);
        }

        public async Task<int> GetNewSystemCode()
        {
            int systemCode;
            var search = await _Client.SearchAsync<NotificationDocument>(s =>
            s.Index($"{IndexName}-*")
            .Size(1)
             .Source(s => s
                .Includes(i => i
                    .Field(f => f.SystemCode)))
            .Sort(s => s
                .Descending(d => d.SystemCode))

            );

            if (search.IsValid && search.Documents.FirstOrDefault() != null)
                systemCode = ++search.Documents.FirstOrDefault().SystemCode;
            else
                systemCode = 10000;

            return systemCode;
        }

        public async Task<bool> AddNotification(string systemCode,string samplePayload, NotificationFields notification)
        {
            notification.SamplePayload = LoggingRepo.ObjectConverter.ContentType(samplePayload,notification.ContentType);

            var update = await _Client.UpdateByQueryAsync<NotificationDocument>(s =>
                s.Index($"{IndexName}-*")

                .Script(q => q
                        .Source("ctx._source['fields'].add(params.notificationStr);")
                        .Params(p => p.Add("notificationStr", notification)))
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.SystemCode)
                             .Query(systemCode)))
                 );

            return update.Updated >= 1;

        }

        public async Task<bool> UpdateNotification(string samplePayload, NotificationFields notification)
        {
            string script = @"for (int i=0;i<ctx._source.fields.size();i++) {
                                if (ctx._source.fields[i].ReferenceNumber == params.referenceNumber) {
                                    ctx._source.fields[i].Name=params.name;
                                    ctx._source.fields[i].ArName=params.arName;
                                    ctx._source.fields[i].SamplePayload=params.samplePayload;
                                    ctx._source.fields[i].Type=params.type; 
                                    ctx._source.fields[i].ContentType=params.contentType; 
                                    }
            }";

            notification.SamplePayload = LoggingRepo.ObjectConverter.ContentType(samplePayload, notification.ContentType);

            var update = await _Client.UpdateByQueryAsync<NotificationDocument>(s =>
                s.Index($"{IndexName}-*")

                .Script(q => q
                        .Source(script)
                        .Params(p => { p.Add("referenceNumber", notification.ReferenceNumber); p.Add("arName", notification.ArName); p.Add("name", notification.Name); p.Add("samplePayload", notification.SamplePayload); p.Add("type", notification.Type); p.Add("contentType", notification.ContentType); return p; }))
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Suffix("fields.ReferenceNumber"))
                             .Query(notification.ReferenceNumber)))
                 );

            return update.Updated >= 1;
        }

        public async Task<bool> UpdateSystemNotification(string systemName, string systemArName, string systemCode)
        {
            var update = await _Client.UpdateByQueryAsync<NotificationDocument>(s =>
                s.Index($"{IndexName}-*")

                .Script(q => q
                        .Source("ctx._source.SystemName=systemName;ctx._source.SystemArName=systemArName;")
                        .Params(p => { p.Add("systemName", systemName); p.Add("systemArName", systemArName);  return p; }))
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.SystemCode)
                             .Query(systemCode)))
                 );

            return update.Updated >= 1;
        }
    }
}