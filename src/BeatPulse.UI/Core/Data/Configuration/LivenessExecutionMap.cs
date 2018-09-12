using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeatPulse.UI.Core.Data.Configuration
{
    class LivenessExecutionMap
        : IEntityTypeConfiguration<LivenessExecution>
    {
        public void Configure(EntityTypeBuilder<LivenessExecution> builder)
        {
            builder.Property(le => le.OnStateFrom)
                .IsRequired(true);

            builder.Property(le => le.LastExecuted)
                .IsRequired(true);

            builder.Property(le => le.Status)
                .HasMaxLength(50)
                .IsRequired(true);

            builder.Property(le => le.IsHealthy)
               .IsRequired(true);

            builder.Property(le => le.LivenessUri)
                .HasMaxLength(500)
                .IsRequired(true);

            builder.Property(le => le.LivenessName)
               .HasMaxLength(500)
               .IsRequired(true);

            builder.Property(le => le.LivenessResult)
                .HasMaxLength(2000)
                .IsRequired(true);

            builder.Property(le => le.DiscoveryService)
                .HasMaxLength(50);

            builder.HasMany(le => le.History)
                .WithOne();
        }
    }
}
