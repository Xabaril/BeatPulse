﻿using System.Collections.Generic;

namespace BeatPulse.UI.Core
{
    interface IUIResourcesReader
    {
        IEnumerable<UIResource> UIResources { get; }
    }
}