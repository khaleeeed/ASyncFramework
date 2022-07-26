using ASyncFramework.Domain.Entities;
using ASyncFramework.Domain.Interface;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ASyncFramework.Infrastructure.Persistence.DapperRepo.Repository
{
    public class TargetFailuerRepository : ITargetFailuerRepository
    {
        private readonly string _ConnectionString;

        public TargetFailuerRepository(IConfiguration configuration)
        {
            _ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<bool> Add(TargetFailuerEntity entity)
        {

            entity.CreationDate = DateTime.Now;
            var sql = $"INSERT INTO TargetFailuer ({nameof(entity.NotificationId)},{nameof(entity.CallBackUrl)},{nameof(entity.ContentBody)}" +
                $",{nameof(entity.Method)},{nameof(entity.Retry)},{nameof(entity.IsSendSuccessfully)},{nameof(entity.StatusCode)}" +
                $",{nameof(entity.SystemCode)},{nameof(entity.Message)},{nameof(entity.IsProcessing)},{nameof(entity.CreationDate)})" +
                $" values(@{nameof(entity.NotificationId)},@{nameof(entity.CallBackUrl)},@{nameof(entity.ContentBody)},@{nameof(entity.Method)},@{nameof(entity.Retry)},@{nameof(entity.IsSendSuccessfully)},@{nameof(entity.StatusCode)},@{nameof(entity.SystemCode)},@{nameof(entity.Message)},@{nameof(entity.IsProcessing)},@{nameof(entity.CreationDate)});";
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var roweffected = await connection.ExecuteAsync(sql, entity);
            if (roweffected > 0)
                return true;
            else
                return false;
        }

        public async Task<TargetFailuerEntity> FindDocument(string referenceNumber)
        {
            var sql = "SELECT * FROM TargetFailuer where notificationId = @ReferenceNumber ORDER BY id desc";

            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var response = await connection.QueryFirstOrDefaultAsync<TargetFailuerEntity>(sql, new { ReferenceNumber = referenceNumber });
            return response;
        }

        public async Task<(IEnumerable<TargetFailuerEntity> doc, long total)> GetAllDocument(int from)
        {
            var sql = "SELECT NotificationId,CallBackUrl,ContentBody,StatusCode,CreationDate FROM TargetFailuer where IsSendSuccessfully=0 and CreationDate > @CreationDate ORDER BY id desc OFFSET @OFFSET ROWS FETCH NEXT 10 ROWS ONLY";
            var sqlCount = "SELECT count(1) FROM TargetFailuer where IsSendSuccessfully = 0 and CreationDate > @CreationDate ORDER BY 1 desc";

            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var responseTask = connection.QueryAsync<TargetFailuerEntity>(sql, new { OFFSET = from, CreationDate = DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd")});
            var countTask = connection.QueryFirstOrDefaultAsync<long>(sqlCount, new { CreationDate = DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd")});
            return (await responseTask, await countTask);
        }

        public async Task<(IEnumerable<TargetFailuerEntity> doc, long total)> GetAllFaulierDocument(int from)
        {
            var sql = "SELECT notificationId,CallBackUrl,ContentBody,StatusCode,CreationDate,Retry,TimeStampCheck FROM TargetFailuer where  IsProcessing=0 and IsSendSuccessfully = 0 and CreationDate > @CreationDate ORDER BY id desc OFFSET @OFFSET ROWS FETCH NEXT 10 ROWS ONLY";
            var sqlCount = "SELECT count(1) FROM TargetFailuer where IsProcessing=0 and IsSendSuccessfully = 0 and CreationDate > @CreationDate ORDER BY 1 desc";

            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var responseTask = connection.QueryAsync<TargetFailuerEntity>(sql, new { OFFSET = from, CreationDate = DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd") });
            var countTask = connection.QueryFirstOrDefaultAsync<long>(sqlCount, new { CreationDate = DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd") });
            return (await responseTask, await countTask);
        }

        public async Task<(IEnumerable<TargetFailuerEntity> doc, long total)> GetAllFaulierDocumentBySystemCode(int from, string systemCode)
        {
            var sql = "SELECT NotificationId,CallBackUrl,ContentBody,StatusCode,CreationDate,Retry FROM TargetFailuer where IsProcessing=0 and IsSendSuccessfully = 0 and CreationDate > @CreationDate and SystemCode=@SystemCode ORDER BY id desc OFFSET @OFFSET ROWS FETCH NEXT 10 ROWS ONLY";
            var sqlCount = "SELECT count(1) FROM TargetFailuer where IsSendSuccessfully = 0 and CreationDate > @CreationDate and SystemCode=@SystemCode ORDER BY 1 desc";

            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var responseTask = connection.QueryAsync<TargetFailuerEntity>(sql, new { OFFSET = from, CreationDate = DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd"), SystemCode = systemCode });
            var countTask = connection.QueryFirstOrDefaultAsync<long>(sqlCount, new { CreationDate = DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd"), SystemCode = systemCode });
            return (await responseTask, await countTask);
        }

        public async Task<bool> ReverseFaulierProcessing(string referenceNumber)
        {
            var sql = "UPDATE TargetFailuer SET IsProcessing = 0 , Retry = Retry - 1 WHERE NotificationId = @ReferenceNumber;";
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var affectedRows = await connection.ExecuteAsync(sql, new { ReferenceNumber = referenceNumber });
            if (affectedRows > 0)
                return true;
            else
                return false;
        }

        public async Task UpdateFaulier(string referenceNumber, bool isSuccessfully)
        {
            var sql = "UPDATE TargetFailuer SET IsProcessing = 0 , IsSendSuccessfully = @IsSendSuccessfully WHERE notificationid = @ReferenceNumber;";
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var affectedRows = await connection.ExecuteAsync(sql, new { ReferenceNumber = referenceNumber, IsSendSuccessfully = isSuccessfully });
        }

        public async Task<bool> UpdateFaulierProcessing(string referenceNumber,byte[] timeStampCheck)
        {
            var sql = "UPDATE TargetFailuer SET IsProcessing = 1 , Retry = Retry + 1 WHERE NotificationId in (@ReferenceNumber) and TimeStampCheck = @TimeStampCheck ;";
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var affectedRows = await connection.ExecuteAsync(sql, new { ReferenceNumber = referenceNumber.ToArray(), TimeStampCheck= timeStampCheck });

            if (affectedRows > 0)
                return true;
            else
                return false;
        }

        public async Task<string> UpdateFaulierProcessing(List<string> referenceNumber, List<byte[]> timeStampChecks)
        {

            var sql = "UPDATE TargetFailuer SET IsProcessing = 1 , Retry = Retry + 1 output inserted.Id WHERE notificationId in @ReferenceNumber and TimeStampCheck in @timeStampChecks;";
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var affectedRows = await connection.QueryAsync<string>(sql, new { ReferenceNumber = referenceNumber, timeStampChecks });

            if (affectedRows.Count() != referenceNumber.Count())
                return referenceNumber.Except(affectedRows).Aggregate((x,y)=>x+","+y);
            else
                return null;

        }

        public async Task<long> CountForSystem(DateTime from, DateTime to, string systemCode)
        {
            string sqlCount = "select count(id) from TargetFailuer where IsProcessing =0 and IsSendSuccessfully = 0 and CreationDate >  @from and CreationDate < @to and systemCode = @systemCode";

            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var recordsTotal = await connection.QueryFirstOrDefaultAsync<long>(sqlCount, new { from, to, systemCode });
            return recordsTotal;
        }

        public async Task<long> CountForAdmin(DateTime from, DateTime to)
        {
            string sqlCount = "select count(id) from TargetFailuer where IsProcessing =0 and IsSendSuccessfully = 0 and CreationDate >  @from and CreationDate < @to";

            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var recordsTotal = await connection.QueryFirstOrDefaultAsync<long>(sqlCount, new { from, to });
            return recordsTotal;
        }
    }
}