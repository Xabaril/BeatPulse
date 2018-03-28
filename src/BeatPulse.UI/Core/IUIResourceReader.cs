using System.Collections.Generic;

namespace BeatPulse.UI.Core
{
    public interface IUIResourceReader
    {
        IEnumerable<UIResource> GetUIResources { get; }
    }
}