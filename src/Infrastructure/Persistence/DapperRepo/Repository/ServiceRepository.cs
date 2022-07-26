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
    public class ServiceRepository: IServiceRepository
    {
        private readonly string _ConnectionString;

        public ServiceRepository(IConfiguration configuration)
        {
            _ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> Add(ServiceEntity entity)
        {
            var sql = $"INSERT INTO Service ({nameof(entity.ServiceCode)},{nameof(entity.SystemCode)}," +
               $"{nameof(entity.ArDiscription)},{nameof(entity.EnDiscription)},{nameof(entity.CreateDate)})" +
               $"Values(@{nameof(entity.ServiceCode)},@{nameof(entity.SystemCode)},@{nameof(entity.ArDiscription)},@{nameof(entity.EnDiscription)}," +
               $"@{nameof(entity.CreateDate)});";

            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var roweffected = await connection.ExecuteAsync(sql, entity);
            if (roweffected > 0)
                return true;
            else
                return false;
        }

        public async Task<IEnumerable<ServiceEntity>> GetAll(string systemCode)
        {
            string sql = "select * from Service where systemCode= @systemCode;";
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var data = await connection.QueryAsync<ServiceEntity>(sql,new {systemCode});
            return data;
        }

        public bool CheckServiceActive(int serviceCode)
        {
            string sql = "select isActive from Service where serviceCode= @serviceCode;";
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var data = connection.QueryFirstOrDefault<bool?>(sql, new { serviceCode });
            return data ?? true;
        }
    }
}
