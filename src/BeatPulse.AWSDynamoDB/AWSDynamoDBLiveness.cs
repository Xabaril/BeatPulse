using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using BeatPulse.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.AWS.DynamoDB
{
    public class AWSDynamoDBLiveness : IBeatPulseLiveness
    {
        public string Name => nameof(AWSDynamoDBLiveness);
        public string Path { get; }
        private readonly AWSDynamoDBOptions _options;

        public AWSDynamoDBLiveness(AWSDynamoDBOptions options, string defaultPath)
        {
            Path = defaultPath ?? throw new ArgumentNullException(nameof(defaultPath));
            EnsureValidOptions(options);
            _options = options;
        }

        public async Task<(string, bool)> IsHealthy(HttpContext context, bool isDevelopment, CancellationToken cancellationToken = default)
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
                var message = !isDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, Name)
                    : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }
        }

        private void EnsureValidOptions(AWSDynamoDBOptions options)
        {
            if (string.IsNullOrEmpty(options.AccessKey)) throw new ArgumentNullException(nameof(AWSDynamoDBOptions.AccessKey));
            if (string.IsNullOrEmpty(options.SecretKey)) throw new ArgumentNullException(nameof(AWSDynamoDBOptions.SecretKey));
            if(options.RegionEndpoint == null) throw new ArgumentNullException(nameof(AWSDynamoDBOptions.RegionEndpoint));
        }
    }
}
