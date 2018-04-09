using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse.UI.Core.Data.Configuration
{
    public class LivenessFailureNotificationsMap
        : IEntityTypeConfiguration<LivenessFailureNotification>
    {
        public void Configure(EntityTypeBuilder<LivenessFailureNotification> builder)
        {
            builder.Property(lf => lf.LivenessName)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(lf => lf.LastNotified)
                .IsRequired();
        }
    }
}
