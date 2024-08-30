using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace MyFinance.Domain.Queries
{
    public class QueriesDbContext : DbContext
    {
        public QueriesDbContext(DbContextOptions<QueriesDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder) { }

        public async Task<(bool CanConnect, string ErrorMessage)> TryConnectionAsync()
        {
            try
            {
                var canConnect = await this.Database.CanConnectAsync();
                if (canConnect)
                    return (true, null);
                await this.Database.OpenConnectionAsync();
                this.Database.CloseConnection();
                return (false, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}