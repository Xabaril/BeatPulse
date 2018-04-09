using BeatPulse.UI.Core.Data.Configuration;
using Microsoft.EntityFrameworkCore;

namespace BeatPulse.UI.Core.Data
{
    class LivenessDb
        : DbContext
    {
        public LivenessDb(DbContextOptions options) : base(options) { }

        public DbSet<LivenessConfiguration> LivenessConfiguration { get; set; }

        public DbSet<LivenessExecutionHistory> LivenessExecutionHistory { get; set; }

        public DbSet<LivenessFailureNotification> LivenessFailuresNotifications { get; set; }

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
            modelBuilder.ApplyConfiguration(new LivenessExecutionHistoryMap());
        }
    }
}
