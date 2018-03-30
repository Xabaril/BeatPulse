using System.Threading.Tasks;

namespace BeatPulse.UI.Core
{
    interface ILivenessFailureNotifier
    {
        Task NotifyFailure(string livenessUri,string content);
    }
}
