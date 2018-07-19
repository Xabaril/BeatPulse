using BeatPulse.Core;
using BeatPulse.DocumentDb;
using System;

namespace BeatPulse
{
    public static class BeatPulseContextEntensions
    {
        public static BeatPulseContext AddDocumentDb(this BeatPulseContext context, Action<DocumentDbOptions> options, string name = nameof(DocumentDbLiveness), string defaultPath = "documentdb")
        {
            var documentDbOptions = new DocumentDbOptions();
            options(documentDbOptions);

            context.AddLiveness(name, setup =>
            {
                setup.UsePath(defaultPath);
                setup.UseLiveness(new DocumentDbLiveness(documentDbOptions));
            });

            return context;
        }
    }
}
