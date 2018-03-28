using System.Collections.Generic;

namespace BeatPulse.UI.Core
{
    public interface IUIResourcesReader
    {
        IEnumerable<UIResource> GetUIResources { get; }
    }
}