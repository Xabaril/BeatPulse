using Amazon;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse.AWS.DynamoDB
{
    public class AWSDynamoDBOptions
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public RegionEndpoint RegionEndpoint { get; set; }
    }
}
