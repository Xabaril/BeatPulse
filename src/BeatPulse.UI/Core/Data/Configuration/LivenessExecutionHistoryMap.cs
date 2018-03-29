using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeatPulse.UI.Core.Data.Configuration
{
    class LivenessExecutionHistoryMap
        : IEntityTypeConfiguration<LivenessExecutionHistory>
    {
        public void Configure(EntityTypeBuilder<LivenessExecutionHistory> builder)
        {
            builder.Property(le => le.ExecutedOn)
                .IsRequired(true);

            builder.Property(le => le.IsHealthy)
                .IsRequired(true);


            builder.Property(le => le.LivenessUri)
                .IsRequired(true);

            builder.Property(le => le.Result)
                .HasMaxLength(2000)
                .IsRequired(true);
        }
    }
}
