using BeatPulse.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.DocumentDb
{
    public class DocumentDbLiveness : IBeatPulseLiveness
    {
        public string Name => nameof(DocumentDbLiveness);

        public string Path { get; }

        private readonly DocumentDbOptions _documentDbOptions = new DocumentDbOptions();

        public DocumentDbLiveness(DocumentDbOptions documentDbOptions, string defaultPath)
        {
            _documentDbOptions.UriEndpoint = documentDbOptions.UriEndpoint ?? throw new ArgumentNullException(nameof(documentDbOptions.UriEndpoint));
            _documentDbOptions.PrimaryKey = documentDbOptions.PrimaryKey ?? throw new ArgumentNullException(nameof(documentDbOptions.PrimaryKey));
            Path = defaultPath ?? throw new ArgumentNullException(nameof(defaultPath));

        }
        public async Task<(string, bool)> IsHealthy(HttpContext context, bool isDevelopment, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var documentDbClient = new DocumentClient(new Uri(_documentDbOptions.UriEndpoint), _documentDbOptions.PrimaryKey))
                {
                    await documentDbClient.OpenAsync();

                    return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
                }
            }
            catch (Exception ex)
            {
                var message = !isDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, Name)
                        : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }
        }
    }
}
