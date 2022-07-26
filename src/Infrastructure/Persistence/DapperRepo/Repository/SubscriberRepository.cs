using ASyncFramework.Domain.Entities;
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
    public class SubscriberRepository : ISubscriberRepository
    {
        private readonly string _ConnectionString;
        private readonly string _Url;
        public SubscriberRepository(IConfiguration configuration)
        {
            _ConnectionString = configuration.GetConnectionString("DefaultConnection");
            _Url = configuration.GetValue<string>("SubscribeUrl");
        }

        public async Task<bool> AddOrUpdate(bool isRunning)
        {
            var checkUrlIsExist = await CheckUrlIsExist();
            if (checkUrlIsExist)
                return await UpdateIsRunning(isRunning);
            else
                return await Add(isRunning);
        }

        public async Task<bool> Add(bool isRunning)
        {
            var sql = $"INSERT  Subscriber (Url,IsRunning,TimeOfTakeConfiguration) Values(@url,@isRunning,@timeOfTakeConfiguration);";
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var Id = await connection.ExecuteAsync(sql, new { url= _Url, isRunning, timeOfTakeConfiguration = DateTime.Now });
            if (Id > 0)
                return true;
            else
                return false;
        }

        public async Task<IEnumerable<string>> GetAllUrl()
        {
            string sql = "select Url from Subscriber";
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var urls= await connection.QueryAsync<string>(sql);
            return urls;
        }

        public async Task<IEnumerable<SubscriberEntity>> GetAll()
        {
            var sql = "select * from Subscriber";
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var response= await connection.QueryAsync<SubscriberEntity>(sql);
            return response;
        }

        public async Task<bool> UpdateIsRunning(bool isRunning)
        {
            var sql = $"update Subscriber set IsRunning= @isRunning where Url = @url";
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var Id = await connection.ExecuteAsync(sql, new { isRunning, url= _Url });
            if (Id > 0)
                return true;
            else
                return false;
        }

        public async Task<bool> UpdateIsRunning(bool isRunning,byte[] timeStampCheck)
        {
            var sql = $"update Subscriber set IsRunning= @isRunning where Url = @url and TimeStampCheck=@timeStampCheck";
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var Id = await connection.ExecuteAsync(sql, new { isRunning, url = _Url , timeStampCheck });
            if (Id > 0)
                return true;
            else
                return false;
        }

        public async Task<bool> UpdateTimeOfTakeConfiguration(DateTime timeOfTakeConfiguration)
        {
            var sql = $"update  Subscriber set TimeOfTakeConfiguration= @timeOfTakeConfiguration where Url = @url";
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var Id = await connection.ExecuteAsync(sql, new { timeOfTakeConfiguration, url= _Url });
            if (Id > 0)
                return true;
            else
                return false;
        }

        public async Task<bool> CheckUrlIsExist()
        {
            string sqlCount = $"select count(id) from Subscriber where Url = @url";

            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var recordsTotal = await connection.QueryFirstOrDefaultAsync<long>(sqlCount, new { url=_Url });
            if (recordsTotal > 0)
                return true;
            else
                return false;
        }
    }
}
