using BP.Manager.Domain.Entity;
using BP.Manager.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;

namespace BP.Manager.Domain.Database
{
    public class BackgroundJobsContext : DbContext
    {
        public const string ConnectionString = "Server=.;Database=BackgroundJobs;Trusted_Connection=True;MultipleActiveResultSets=true";
        public DbSet<BackgroundJobState> BackgroundJobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BackgroundJobState>()
                .Property(e => e.GroupName)
                .IsRequired();

            modelBuilder.Entity<BackgroundJobState>()
                .Property(e => e.Status)
                .IsRequired()
                .HasConversion(
                    v => v.ToString(),
                    v => (BackgroundJobstatus)Enum.Parse(typeof(BackgroundJobstatus), v));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }
    }
}
