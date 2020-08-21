using BP.Manager.Domain.Entity;
using BP.Manager.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;

namespace BP.Manager.Domain.Database
{
    public class TaskStatesContext : DbContext
    {
        public const string ConnectionString = "Server=.;Database=BackgroundTasks;Trusted_Connection=True;MultipleActiveResultSets=true";
        public DbSet<BackgroundTaskEntity> BackgroundTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BackgroundTaskEntity>()
                .Property(e => e.UserName)
                .IsRequired();

            modelBuilder.Entity<BackgroundTaskEntity>()
                .Property(e => e.Status)
                .IsRequired()
                .HasConversion(
                    v => v.ToString(),
                    v => (BackgroundTaskStatus)Enum.Parse(typeof(BackgroundTaskStatus), v));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }
    }
}
