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
    public class DisposeRepository : IDisposeRepository
    {
        private readonly string _ConnectionString;

        public DisposeRepository(IConfiguration configuration)
        {
            _ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> Add(DisposeEntity entity)
        {
            entity.CreationDate = DateTime.Now;
            var sql = $"INSERT INTO dispose Values(@{nameof(entity.NotificationId)},@{nameof(entity.IsActive)},@{nameof(entity.CreationDate)});";
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var Id = await connection.ExecuteAsync(sql, entity);
            if (Id > 0)
                return true;
            else
                return false;
        }

        public async Task<DisposeEntity> FindDocument(string referenceNumber)
        {
            var sql = "SELECT top 1 * FROM dispose WITH (NOLOCK) where NotificationId = @ReferenceNumber and IsActive = 1 ORDER BY id desc";

            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var response = await connection.QueryFirstOrDefaultAsync<DisposeEntity>(sql, new { ReferenceNumber = referenceNumber });
            return response;
        }

    }
}
