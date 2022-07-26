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
    public class PushFailuerRepository : IPushFailuerRepository
    {
        private readonly string _ConnectionString;

        public PushFailuerRepository(IConfiguration configuration)
        {
            _ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> Add(PushFailuerEntity entity)
        {
            entity.CreationDate = DateTime.Now;
            var sql = $"INSERT INTO PushFailuer Values(@{nameof(entity.NotificationId)},@{nameof(entity.CreationDate)},@{nameof(entity.IsActive)},@{nameof(entity.Queues)},@{nameof(entity.Retry)},@{nameof(entity.IsCallBackMessage)},@{nameof(entity.IsFailureMessage)});";
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var rowEffected = await connection.ExecuteAsync(sql, entity);
            if (rowEffected > 0)
                return true;
            else
                return false;
        }

        public async Task<IEnumerable<PushFailuerEntity>> GetAll()
        {
            string sql = $"select top 200 * from PushFailuer join notification on notification.id = PushFailuer.NotificationId where IsActive = 1 order by 1 asc";
            using var connection = new SqlConnection(_ConnectionString);

            var PushFailuerEntities = await connection.QueryAsync<PushFailuerEntity, NotificationEntity, PushFailuerEntity>(sql, (PushFailuerEntity, NotificationEntity) =>
            {
                PushFailuerEntity.Notification = NotificationEntity;
                return PushFailuerEntity;
            });

            return PushFailuerEntities;
        }

        public async Task UpdateIsActive(long id)
        {
            string sqlCount = "update PushFailuer set IsActive = 0  where id = @id";

            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();

            await connection.ExecuteAsync(sqlCount, new { id });
        }
    }
}
