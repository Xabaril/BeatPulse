using BeatPulse.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Elasticsearch
{
    public class ElasticsearchLiveness : IBeatPulseLiveness
    {
        private readonly string _elasticsearchConnectionString;
        private readonly ILogger<ElasticsearchLiveness> _logger;

        public ElasticsearchLiveness(string elasticsearchConnectionString, ILogger<ElasticsearchLiveness> logger = null)
        {
            _elasticsearchConnectionString = elasticsearchConnectionString ?? throw new ArgumentNullException(nameof(elasticsearchConnectionString));
            _logger = logger;
        }

        public async Task<LivenessResult> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                _logger?.LogInformation($"{nameof(ElasticsearchLiveness)} is checking the Elasticsearch host.");

                var client = new ElasticClient(new Uri(_elasticsearchConnectionString));
                var result = await client.PingAsync(cancellationToken:cancellationToken);
                var success = result.ApiCall.HttpStatusCode == StatusCodes.Status200OK;

                return success
                    ? LivenessResult.Healthy()
                    : LivenessResult.UnHealthy(result.ApiCall.OriginalException);

            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(ElasticsearchLiveness)} check fail with the exception {ex.ToString()}.");

                return LivenessResult.UnHealthy(ex);
            }
        }
    }
}
