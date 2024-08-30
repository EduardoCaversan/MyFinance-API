using MyFinance.Domain.Utils;
using Microsoft.EntityFrameworkCore;
using MyFinance.Domain.Commands.Users.Entities;
using MyFinance.Domain.Commands.Users.Entities.DbConfig;

namespace MyFinance.Domain.Commands
{
    public class CommandsDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public List<LogChangedEntry> LogChangedEntries { get; set; }

        public CommandsDbContext(DbContextOptions<CommandsDbContext> options) : base(options)
        {
            LogChangedEntries = new List<LogChangedEntry>();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UserDbConfig());
        }

        public async Task<(bool CanConnect, string ErrorMessage)> TryConnectionAsync()
        {
            try
            {
                var canConnect = await this.Database.CanConnectAsync();
                if (canConnect)
                    return (true, "");
                await this.Database.OpenConnectionAsync();
                this.Database.CloseConnection();
                return (false, "");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {
            var now = DateTimeOffset.Now;
            var entries = ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Unchanged)
                    continue;

                var logChangedEntry = new LogChangedEntry()
                {
                    Id = entry.Property("Id").CurrentValue.ToString(),
                    EntityName = entry.Entity.GetType().Name.ToString(),
                    EntityState = entry.State.ToString()
                };

                foreach (var property in entry.Properties)
                {
                    if (entry.State != EntityState.Added && !property.IsModified)
                        continue;

                    if (entry.State != EntityState.Added && (property.OriginalValue == null || property.OriginalValue.Equals(property.CurrentValue)))
                        continue;

                    if (property.Metadata.Name == "LastModified")
                        continue;

                    logChangedEntry.Properties.Add(new LogChangedProperty()
                    {
                        PropertyName = property.Metadata.Name,
                        OriginalValue = entry.State == EntityState.Added ? null : property.OriginalValue,
                        CurrentValue = property.CurrentValue
                    });
                }

                if (logChangedEntry.Properties.Count == 0)
                    continue;

                if (entry.Entity is ITrackedEntity trEntity && !trEntity.IsOfflineCommand)
                    trEntity.LastModified = now;

                LogChangedEntries.Add(logChangedEntry);
            }
        }
    }
}