using BeatPulse.UI.Core.Data.Configuration;
using Microsoft.EntityFrameworkCore;

namespace BeatPulse.UI.Core.Data
{
    class LivenessDb
        : DbContext
    {
        public DbSet<LivenessConfiguration> LivenessConfigurations { get; set; }

        public DbSet<LivenessExecution> LivenessExecutions { get; set; }

        public DbSet<LivenessFailureNotification> LivenessFailuresNotifications { get; set; }

        public LivenessDb(DbContextOptions<LivenessDb> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=livenessdb");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new LivenessConfigurationMap());
            modelBuilder.ApplyConfiguration(new LivenessExecutionMap());
            modelBuilder.ApplyConfiguration(new LivenessExecutionHistoryMap());
            modelBuilder.ApplyConfiguration(new LivenessFailureNotificationsMap());
        }
    }
}
