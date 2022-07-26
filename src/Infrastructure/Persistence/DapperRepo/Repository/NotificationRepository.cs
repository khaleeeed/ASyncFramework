using ASyncFramework.Domain.Entities;
using ASyncFramework.Domain.Enums;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Interface.Repository;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Infrastructure.Persistence.DapperRepo.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IInfrastructureLogger<NotificationRepository> _logger;

        private readonly string _ConnectionString;

        private readonly string _FinshStatusID = $"{(int)MessageLifeCycle.ValidationError},{(int)MessageLifeCycle.Succeeded}," +
                $"{(int)MessageLifeCycle.SucceededWithoutCallBack},{(int)MessageLifeCycle.MessageDisposed}";

        private readonly string _ProccessingStatusID = $"{(int)MessageLifeCycle.ValidationError},{(int)MessageLifeCycle.CallBackUsedAllRetries},{(int)MessageLifeCycle.Succeeded}," +
                $"{(int)MessageLifeCycle.SucceededWithoutCallBack},{(int)MessageLifeCycle.TargetUsedAllRetries},{(int)MessageLifeCycle.MessageDisposed}";
        public NotificationRepository(IConfiguration configuration, IInfrastructureLogger<NotificationRepository> logger)
        {
            _ConnectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }


        public long Add(NotificationEntity entity)
        {
            entity.CreationDate = DateTime.Now;
            var sql = $"INSERT INTO Notification OUTPUT INSERTED.[Id] Values(@{nameof(entity.Request)},@{nameof(entity.Header)},@{nameof(entity.Hash)},@{nameof(entity.ExtraInfo)},@{nameof(entity.SystemCode)},@{nameof(entity.TargetUrl)},@{nameof(entity.CallBackUrl)},@{nameof(entity.CreationDate)},@{nameof(entity.MessageLifeCycleId)},@{nameof(entity.ServiceCode)});";
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var Id = connection.QuerySingle<long>(sql, entity);
            if (Id > 0)
                return Id;
            else
                return Id;
        }

        // count all notification
        public async Task<long> CountForSystem(DateTime from, DateTime to, string systemCode)
        {
            string sqlCount = "select count(id) from Notification where CreationDate >  @from and CreationDate < @to and systemCode = @systemCode";

            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var recordsTotal = await connection.QueryFirstOrDefaultAsync<long>(sqlCount, new { from, to, systemCode });
            return recordsTotal;

        }

        // count all notification
        public async Task<long> CountForAdmin(DateTime from, DateTime to)
        {
            string sqlCount = "select count(id) from Notification where CreationDate >  @from and CreationDate < @to";

            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var recordsTotal = await connection.QueryFirstOrDefaultAsync<long>(sqlCount, new { from, to });
            return recordsTotal;
        }

        // retrive all notification data
        public async Task<(IEnumerable<NotificationEntity> doc, long total)> DetailsForAdmin(int from, DateTime fromDate, DateTime toDate)
        {
            IEnumerable<NotificationEntity> data = new List<NotificationEntity>();

            string sqlCount = "select count(id) from Notification where CreationDate >  @from and CreationDate < @to";
            string sql = "select id,TargetUrl,CallBackUrl,CreationDate,messageLifeCycleId from Notification where CreationDate >  @from and CreationDate < @to order by id desc  OFFSET @OFFSET ROWS FETCH NEXT 10 ROWS ONLY";
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();

            var countTask = connection.QueryFirstOrDefaultAsync<long>(sqlCount, new { from = fromDate, to = toDate });
            var responseTask = connection.QueryAsync<NotificationEntity>(sql, new { OFFSET = from, from = fromDate, to = toDate });
            var recordsTotal = await countTask;
            if (recordsTotal != 0)
                data = await responseTask;
            return (data, recordsTotal);

        }

        // retrive all notification data
        public async Task<(IEnumerable<NotificationEntity> doc, long total)> DetailsForSystem(int from, DateTime fromDate, DateTime toDate, string systemCode)
        {
            IEnumerable<NotificationEntity> data = new List<NotificationEntity>();

            string sqlCount = "select count(id) from Notification where CreationDate >  @from and CreationDate < @to and systemCode = @systemCode";
            string sql = "select id,TargetUrl,CallBackUrl,CreationDate,messageLifeCycleId from Notification where CreationDate >  @from and CreationDate < @to and systemCode ='" + systemCode + "'order by id desc  OFFSET @OFFSET ROWS FETCH NEXT 10 ROWS ONLY";
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();

            var countTask = connection.QueryFirstOrDefaultAsync<long>(sqlCount, new { from = fromDate, to = toDate, systemCode });
            var responseTask = connection.QueryAsync<NotificationEntity>(sql, new { OFFSET = from, from = fromDate, to = toDate });
            var recordsTotal = await countTask;
            if (recordsTotal != 0)
                data = await responseTask;
            return (data, recordsTotal);

        }

        public async Task<(IEnumerable<NotificationEntity> doc, long total)> DetailsInProcessForAdmin(int from, DateTime fromDate, DateTime toDate)
        {
            IEnumerable<NotificationEntity> data = new List<NotificationEntity>();

            string sqlCount = $"select count(id) from Notification where messageLifeCycleId not in ({_ProccessingStatusID}) and CreationDate >  @from and CreationDate < @to";
            string sql = $"select id,TargetUrl,CallBackUrl,CreationDate,messageLifeCycleId from Notification where  messageLifeCycleId not in ({_ProccessingStatusID}) and CreationDate >  @from and CreationDate < @to  order by id desc  OFFSET @OFFSET ROWS FETCH NEXT 10 ROWS ONLY";
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();

            var countTask = connection.QueryFirstOrDefaultAsync<long>(sqlCount, new { from = fromDate, to = toDate });
            var responseTask = connection.QueryAsync<NotificationEntity>(sql, new { OFFSET = from, from = fromDate, to = toDate });
            var recordsTotal = await countTask;
            if (recordsTotal != 0)
                data = await responseTask;
            return (data, recordsTotal);

        }

        public async Task<(IEnumerable<NotificationEntity> doc, long total)> DetailsInProcessForSystem(int from, DateTime fromDate, DateTime toDate, string systemCode)
        {

            IEnumerable<NotificationEntity> data = new List<NotificationEntity>();

            string sqlCount = $"select count(id) from Notification where messageLifeCycleId not in ({_ProccessingStatusID}) and CreationDate >  @from and CreationDate < @to and systemCode = @systemCode";
            string sql = $"select id,TargetUrl,CallBackUrl,CreationDate,messageLifeCycleId from Notification where  messageLifeCycleId not in ({_ProccessingStatusID}) and CreationDate >  @from and CreationDate < @to and systemCode = @systemCode order by id desc  OFFSET @OFFSET ROWS FETCH NEXT 10 ROWS ONLY";
            using var connection = new SqlConnection(_ConnectionString);

            var countTask = connection.QueryFirstOrDefaultAsync<long>(sqlCount, new { from = fromDate, to = toDate, systemCode });
            var responseTask = connection.QueryAsync<NotificationEntity>(sql, new { OFFSET = from, from = fromDate, to = toDate, systemCode });
            var recordsTotal = await countTask;
            if (recordsTotal != 0)
                data = await responseTask;
            return (data, recordsTotal);
        }

        public async Task<long> CountInProcessForSystem(DateTime from, DateTime to, string systemCode)
        {
            string sqlCount = $"select count(id) from Notification where messageLifeCycleId not in ({_ProccessingStatusID}) and CreationDate >  @from and CreationDate < @to and systemCode = @systemCode";

            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var recordsTotal = await connection.QueryFirstOrDefaultAsync<long>(sqlCount, new { from, to, systemCode });
            return recordsTotal;
        }

        public async Task<long> CountInProcessForAdmin(DateTime from, DateTime to)
        {
            string sqlCount = $"select count(id) from Notification where messageLifeCycleId not in ({_ProccessingStatusID}) and CreationDate >  @from and CreationDate < @to";

            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var recordsTotal = await connection.QueryFirstOrDefaultAsync<long>(sqlCount, new { from, to });
            return recordsTotal;
        }

        public void UpdateStatusId(string referenceNumber, MessageLifeCycle messageLifeCycle)
        {
            try
            {
                string sqlCount = "update Notification set messageLifeCycleId = @messageLifeCycleId  where id = @id";

                using var connection = new SqlConnection(_ConnectionString);
                connection.Open();
                long.TryParse(referenceNumber, out long id);
                var recordsAffected = connection.Execute(sqlCount, new { id, messageLifeCycleId = Convert.ToInt32(messageLifeCycle) });

            }
            catch (Exception ex)
            {
                _logger.LogError(DateTime.Now, ex, MessageLifeCycle.ExceptoinWhenUpdateStatus, referenceNumber);
            }
        }

        public async Task<long> CountFinshForSystem(DateTime from, DateTime to, string systemCode)
        {
            string sqlCount = $"select count(id) from Notification where messageLifeCycleId in ({_FinshStatusID}) and CreationDate >  @from and CreationDate < @to and systemCode = @systemCode";

            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var recordsTotal = await connection.QueryFirstOrDefaultAsync<long>(sqlCount, new { from, to, systemCode });
            return recordsTotal;
        }

        public async Task<long> CountFinshForAdmin(DateTime from, DateTime to)
        {
            string sqlCount = $"select count(id) from Notification where messageLifeCycleId in ({_FinshStatusID}) and CreationDate >  @from and CreationDate < @to";

            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var recordsTotal = await connection.QueryFirstOrDefaultAsync<long>(sqlCount, new { from, to });
            return recordsTotal;
        }

        public (bool IsSendBefore, string ReferenceNumber) CheckIfRequestSendBefore(string hash, string id)
        {
            var sql = "select top 1 id from Notification WITH (NOLOCK) where hash = @hash and id <> @Id order by 1 desc";
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            long.TryParse(id, out long Id);
            var response = connection.QueryFirstOrDefault<long>(sql, new { hash, Id });
            return (response != default, response.ToString());
        }
    }
}