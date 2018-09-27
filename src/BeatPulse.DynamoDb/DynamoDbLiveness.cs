﻿using Amazon.DynamoDBv2;
using Amazon.Runtime;
using BeatPulse.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.DynamoDb
{
    public class DynamoDbLiveness
        : IBeatPulseLiveness
    {
        private readonly DynamoDBOptions _options;
        private readonly ILogger<DynamoDbLiveness> _logger;

        public DynamoDbLiveness(DynamoDBOptions options, ILogger<DynamoDbLiveness> logger = null)
        {
            if (string.IsNullOrEmpty(options.AccessKey)) throw new ArgumentNullException(nameof(DynamoDBOptions.AccessKey));
            if (string.IsNullOrEmpty(options.SecretKey)) throw new ArgumentNullException(nameof(DynamoDBOptions.SecretKey));
            if (options.RegionEndpoint == null) throw new ArgumentNullException(nameof(DynamoDBOptions.RegionEndpoint));

            _options = options;
            _logger = logger;
        }

        public async Task<LivenessResult> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger?.LogDebug($"{nameof(DynamoDbLiveness)} is checking DynamoDb database availability.");

                var credentials = new BasicAWSCredentials(_options.AccessKey, _options.SecretKey);
                var client = new AmazonDynamoDBClient(credentials, _options.RegionEndpoint);

                await client.ListTablesAsync();

                _logger?.LogDebug($"The {nameof(DynamoDbLiveness)} check success.");

                return LivenessResult.Healthy();
            }
            catch (Exception ex)
            {
                _logger?.LogDebug($"The {nameof(DynamoDbLiveness)} check fail with the exception {ex.ToString()}.");

                return LivenessResult.UnHealthy(ex);
            }
        }
    }
}
