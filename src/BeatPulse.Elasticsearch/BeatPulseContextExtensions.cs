using BeatPulse.Core;
using BeatPulse.Elasticsearch;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddElasticsearch(this BeatPulseContext context, string elasticsearchConnectionString, string name = nameof(ElasticsearchLiveness), string defaultPath = "elasticsearch")
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UsePath(defaultPath);
                setup.UseLiveness(new ElasticsearchLiveness(elasticsearchConnectionString));
            });
        }
    }
}
