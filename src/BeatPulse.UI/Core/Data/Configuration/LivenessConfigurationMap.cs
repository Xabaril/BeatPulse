using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeatPulse.UI.Core.Data.Configuration
{
    class LivenessConfigurationMap
        : IEntityTypeConfiguration<LivenessConfiguration>
    {
        public void Configure(EntityTypeBuilder<LivenessConfiguration> builder)
        {
            builder.HasKey(lc => lc.Id);

            builder.Property(lc => lc.LivenessUri)
                .IsRequired(true)
                .HasMaxLength(500);

            builder.Property(lc => lc.LivenessName)
                .IsRequired(true)
                .HasMaxLength(500);

            builder.Property(lc => lc.DiscoveryService)
                .HasMaxLength(100);
        }
    }
}
