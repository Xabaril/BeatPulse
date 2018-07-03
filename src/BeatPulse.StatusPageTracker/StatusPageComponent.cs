namespace BeatPulse.StatusPageTracker
{
    public class StatusPageComponent
    {
        public string ApiKey { get; set; }

        public string PageId { get; set; }

        public string ComponentId { get; set; }

        public string IncidentName { get; set; }

        public StatusPageComponent()
        {
            ApiKey = "your-api-key-here";
            PageId = "your-pageid-here";
            ComponentId = "your-componentid-here";
            IncidentName = "your-incidentname-here";

        }
    }
}
