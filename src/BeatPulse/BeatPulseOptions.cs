namespace BeatPulse
{
    public class BeatPulseOptions
    {
        public bool DetailedOutput { get; set; }

        public string BeatPulsePath { get; set; }

        public BeatPulseOptions()
        {
            DetailedOutput = true;
            BeatPulsePath = BeatPulseKeys.BEATPULSE_DEFAULT_PATH;
        }
    }
}
