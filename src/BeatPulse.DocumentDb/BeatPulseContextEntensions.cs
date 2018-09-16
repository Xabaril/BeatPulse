using System;
using BeatPulse.DocumentDb;
using Microsoft.Extensions.DependencyInjection;

namespace BeatPulse
{
    public static class HealthChecksBuilderEntensions
    {
        public static IHealthChecksBuilder AddDocumentDb(this IHealthChecksBuilder builder, Action<DocumentDbOptions> options, string name = nameof(DocumentDbLiveness), string defaultPath = "documentdb")
        {
            var documentDbOptions = new DocumentDbOptions();
            options(documentDbOptions);

            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new DocumentDbLiveness(documentDbOptions));
        }
    }
}
