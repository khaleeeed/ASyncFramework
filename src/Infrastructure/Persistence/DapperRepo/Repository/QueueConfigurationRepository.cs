using ASyncFramework.Domain.Common;
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
    public class QueueConfigurationRepository : IQueueConfigurationRepository
    {
        private readonly string _ConnectionString;

        public QueueConfigurationRepository(IConfiguration configuration)
        {
            _ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> Add(QueueConfigurations entity)
        {
            var sql = $"INSERT INTO QueueConfiguration ({nameof(entity.ID)},{nameof(entity.QueueRetry)}," +
               $"{nameof(entity.Dealy)},{nameof(entity.QueueName)},{nameof(entity.ExhangeName)}," +
               $"{nameof(entity.IsAutoMapping)},{nameof(entity.NumberOfInstance)},{nameof(entity.ExhangeType)})" +
               $"Values(@{nameof(entity.ID)},@{nameof(entity.QueueRetry)},@{nameof(entity.Dealy)},@{nameof(entity.QueueName)}," +
               $"@{nameof(entity.ExhangeName)},@{nameof(entity.IsAutoMapping)},@{nameof(entity.NumberOfInstance)},@{nameof(entity.ExhangeType)});";

            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var roweffected = await connection.ExecuteAsync(sql, entity);
            if (roweffected > 0)
                return true;
            else
                return false;
        }       

        public async Task<IEnumerable<QueueConfigurations>> GetAll()
        {
          
            string sql = "select * from QueueConfiguration";
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var data = await connection.QueryAsync<QueueConfigurations>(sql);
            return data;
        }

        public async Task<bool> Update(int id,int queueRetry,bool isAutoMapping,int numberOfInstance,byte[] timeStampCheck)
        {
            var sql = $"update QueueConfiguration set QueueRetry= @queueRetry,IsAutoMapping=@isAutoMapping,NumberOfInstance=@numberOfInstance where id = @id and TimeStampCheck= @timeStampCheck";
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var Id = await connection.ExecuteAsync(sql, new { id,queueRetry,isAutoMapping,numberOfInstance,timeStampCheck });
            if (Id > 0)
                return true;
            else
                return false;
        }

        public async Task<QueueConfigurations> Get(int id)
        {
            string sql = "select * from QueueConfiguration where id=@id";
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var data = await connection.QuerySingleAsync<QueueConfigurations>(sql, new { id });
            return data;
        }

    }
}