using Amazon;

namespace BeatPulse.DynamoDb
{
    public class DynamoDBOptions
    {
        public string AccessKey { get; set; }

        public string SecretKey { get; set; }

        public RegionEndpoint RegionEndpoint { get; set; }
    }
}
