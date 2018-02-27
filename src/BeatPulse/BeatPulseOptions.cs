namespace BeatPulse
{
    public class BeatPulseOptions
    {
        public bool DetailedOutput { get; set; }

        public string BeatPulsePath { get; set; }

        public int Timeout { get; set; }

        public BeatPulseOptions()
        {
            DetailedOutput = false;
            BeatPulsePath = BeatPulseKeys.BEATPULSE_DEFAULT_PATH;
            Timeout = -1; //by default wait infinitely
        }
    }
}
