namespace BeatPulse.UI.Core.Data
{
    class LivenessConfiguration
    {
        public int Id { get; set; }

        public string LivenessUri { get; set; }

        public string LivenessName { get; set; }

        public void Deconstruct(out string uri, out string name)
        {
            uri = this.LivenessUri;
            name = this.LivenessName;
        }
    }
}
