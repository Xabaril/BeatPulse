using Amazon.DynamoDBv2;
using Amazon.Runtime;
using BeatPulse.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.DynamoDb
{
    public class DynamoDbLiveness
        : IBeatPulseLiveness
    {
        private readonly DynamoDBOptions _options;

        public DynamoDbLiveness(DynamoDBOptions options)
        {
            if (string.IsNullOrEmpty(options.AccessKey)) throw new ArgumentNullException(nameof(DynamoDBOptions.AccessKey));
            if (string.IsNullOrEmpty(options.SecretKey)) throw new ArgumentNullException(nameof(DynamoDBOptions.SecretKey));
            if (options.RegionEndpoint == null) throw new ArgumentNullException(nameof(DynamoDBOptions.RegionEndpoint));

            _options = options;
        }

        public async Task<(string, bool)> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var credentials = new BasicAWSCredentials(_options.AccessKey, _options.SecretKey);
                var client = new AmazonDynamoDBClient(credentials, _options.RegionEndpoint);
                await client.ListTablesAsync();

                return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
            }
            catch (Exception ex)
            {
                var message = !context.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name)
                    : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }
        }
    }
}
