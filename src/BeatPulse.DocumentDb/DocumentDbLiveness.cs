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

        private readonly DocumentDbOptions _documentDbOptions = new DocumentDbOptions();

        public DocumentDbLiveness(DocumentDbOptions documentDbOptions)
        {
            _documentDbOptions.UriEndpoint = documentDbOptions.UriEndpoint ?? throw new ArgumentNullException(nameof(documentDbOptions.UriEndpoint));
            _documentDbOptions.PrimaryKey = documentDbOptions.PrimaryKey ?? throw new ArgumentNullException(nameof(documentDbOptions.PrimaryKey));

        }
        public async Task<(string, bool)> IsHealthy(LivenessExecutionContext livenessContext, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var documentDbClient = new DocumentClient(
                    new Uri(_documentDbOptions.UriEndpoint), 
                    _documentDbOptions.PrimaryKey))
                {
                    await documentDbClient.OpenAsync();

                    return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
                }
            }
            catch (Exception ex)
            {
                var message = !livenessContext.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, livenessContext.Name)
                        : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }
        }
    }
}
