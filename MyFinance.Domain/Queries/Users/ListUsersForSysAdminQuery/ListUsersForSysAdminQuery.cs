using Dapper;
using Microsoft.EntityFrameworkCore;
using MyFinance.Domain.Utils;

namespace MyFinance.Domain.Queries.Users.ListUsersForSysAdminQuery
{
    public class ListUsersForSysAdminQuery : AbstractQuery<UserViewModel>
    {
        public override async Task<QueryResult<UserViewModel>> ExecuteAsync(QueriesHandler queriesHandler)
        {
            var sql = @"
                SELECT
                    [u].[id]
                    ,[u].[name]
                    ,[u].[email]
                    ,[u].[mobile_number]
                FROM [dbo].[users] [u]
                WHERE [u].[removed] = 0
                    AND [u].[is_sys_admin] = 0
            ";

            using var connection = queriesHandler.QueriesDbContext.Database.GetDbConnection();
            var data = await connection.QueryAsync<UserViewModel>(sql);
            return new QueryResult<UserViewModel>(data);
        }

        public override async Task<bool> HasPermissionAsync(QueriesHandler queriesHandler)
        {
            return await Task.FromResult(queriesHandler.CurrentRequest.IsSysAdmin);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}