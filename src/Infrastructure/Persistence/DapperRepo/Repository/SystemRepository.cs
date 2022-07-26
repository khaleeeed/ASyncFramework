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
    public class SystemRepository : ISystemRepository
    {
        private readonly string _ConnectionString;

        public SystemRepository(IConfiguration configuration)
        {
            _ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public SystemEntity GetSystem(string systemCode)
        {
            string sql = "select * from system where systemCode= @systemCode;";
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var data = connection.QueryFirstOrDefault<SystemEntity>(sql, new { systemCode });
            return data;
        }

        public async Task<bool> Add(SystemEntity entity)
        {
            var sql = $"INSERT INTO system ({nameof(entity.SystemCode)},{nameof(entity.EnSystemName)}," +
               $"{nameof(entity.ArSystemName)},{nameof(entity.IsActive)})" +
               $"Values(@{nameof(entity.SystemCode)},@{nameof(entity.EnSystemName)},@{nameof(entity.ArSystemName)},@{nameof(entity.IsActive)});";

            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var roweffected = await connection.ExecuteAsync(sql, entity);
            if (roweffected > 0)
                return true;
            else
                return false;
        }

        public async Task<bool> Update(int systemCode, string enSystemName, string arSystemName, bool isActive, bool hasCustomQueue)
        {
            var sql = $"update [system] set EnSystemName=@enSystemName,ArSystemName=@arSystemName,IsActive=@isActive,HasCustomQueue=@hasCustomQueue where SystemCode = @systemCode";
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var Id = await connection.ExecuteAsync(sql, new {enSystemName, arSystemName, isActive, hasCustomQueue, systemCode });
            if (Id > 0)
                return true;
            else
                return false;
        }

        public async Task<IEnumerable<SystemEntity>> GetAll()
        {
            string sql = "select * from System";
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var data = await connection.QueryAsync<SystemEntity>(sql);
            return data;
        }

        public (bool, bool) CheckSystemActive(string systemCode)
        {
            string sql = "select isActive,HasCustomQueue from system where systemCode= @systemCode;";
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var data = connection.QueryFirstOrDefault<SystemEntity>(sql, new { systemCode });
            if (data == null)
                return (true, false);
            return (data.IsActive, data.HasCustomQueue );
        }

      
    }
}