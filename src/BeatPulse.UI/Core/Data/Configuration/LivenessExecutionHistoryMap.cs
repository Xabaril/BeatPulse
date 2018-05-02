using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeatPulse.UI.Core.Data.Configuration
{
    class LivenessExecutionHistoryMap
        : IEntityTypeConfiguration<LivenessExecutionHistory>
    {
        public void Configure(EntityTypeBuilder<LivenessExecutionHistory> builder)
        {
            builder.Property(le => le.On)
                .IsRequired(true);

            builder.Property(le => le.Status)
                .HasMaxLength(50)
                .IsRequired(true);
        }
    }
}
