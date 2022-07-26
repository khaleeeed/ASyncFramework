using ASyncFramework.Domain.Entities;
using ASyncFramework.Domain.Interface.Repository;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Infrastructure.Persistence.DapperRepo.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly string _ConnectionString;

        public UserRepository(IConfiguration configuration)
        {
            _ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public UserEntity GetUser(string userName)
        {
            string sql = @"select * from [User] 
                               left join[role] on[role].id = [user].roleid
                               left join[system] on[system].id = [user].systemId
                               where UserName = @userName and [user].IsActive=1";

            using var connection = new SqlConnection(_ConnectionString);

            var user = connection.Query<UserEntity, RoleEntity , SystemEntity, UserEntity>(sql, (userEntity, roleEntity, systemEntity) =>
             {
                 userEntity.System = systemEntity;
                 userEntity.Role = roleEntity;
                 return userEntity;
             }, new { userName });

            return user.FirstOrDefault();
        }

        public async Task<bool> Add(string userName , string systemCode, string roleName)
        {
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var sql = "select id from [system] where systemcode = @systemCode";
            var systemTask = connection.QueryFirstAsync<int>(sql, new {systemCode});
            sql = "select id from [role] where roleName = @roleName";
            var roleTask = connection.QueryFirstAsync<int>(sql, new { roleName });
            var system = await systemTask;
            var role = await roleTask;
            var entity = new UserEntity
            {
                RoleId = role,
                IsActive = true,
                SystemId = system,
                UserName = userName
            };
            sql = $"INSERT INTO [user] Values(@UserName,@RoleId,@SystemId,@IsActive);";
            var rowEffected = await connection.ExecuteAsync(sql, entity);
            if (rowEffected > 0)
                return true;
            else
                return false;
        }

        public async Task<bool> RemoveUser(string userName)
        {
            var array = userName.Split('@');
            if (array != null)
                userName = array[0];

            var sql = $"update [user] set isActive= 0 where UserName = @userName";
            using var connection = new SqlConnection(_ConnectionString);
            connection.Open();
            var Id = await connection.ExecuteAsync(sql, new { userName });
            if (Id > 0)
                return true;
            else
                return false;
        }
    }
}