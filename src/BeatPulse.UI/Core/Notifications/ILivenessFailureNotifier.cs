using System.Threading.Tasks;

namespace BeatPulse.UI.Core.Notifications
{
    interface ILivenessFailureNotifier
    {
        Task NotifyWakeDown(string livenessName, string message);

        Task NotifyWakeUp(string livenessName);
    }
}
