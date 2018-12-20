using System;
using System.Threading.Tasks;

namespace BeatPulse.UI.Core.Notifications
{
    interface ILivenessFailureNotifier : IDisposable
    {
        Task NotifyDown(string livenessName, string message);

        Task NotifyWakeUp(string livenessName);
    }
}
