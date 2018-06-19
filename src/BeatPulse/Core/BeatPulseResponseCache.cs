using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse.Core
{
    public class BeatPulseResponseCache
    {
        private readonly ConcurrentDictionary<string, OutputLivenessMessage> _data;
        private readonly BeatPulseOptions _options;
        public BeatPulseResponseCache(BeatPulseOptions options)
        {
            _options = options;
            _data = new ConcurrentDictionary<string, OutputLivenessMessage>();
        }

        public bool TryGet(string path, out OutputLivenessMessage message)
        {
            message = null;

            if (_options.CacheOutput
                && _options.CacheMode.UseServerMemory()
                && _data.TryGetValue(path, out message))
            {
                var seconds = (DateTime.UtcNow - message.EndAtUtc).TotalSeconds;

                if (_options.CacheDuration > seconds)
                {
                    return true;
                }
                else
                {
                    _data.TryRemove(path, out OutputLivenessMessage removed);
                    return false;
                }
            }

            return false;
        }

        public void TryAddIfNeeded(string beatPulsePath, OutputLivenessMessage output)
        {
            if (_options.CacheMode.UseServerMemory() && _options.CacheOutput)
            {
                _data.TryAdd(beatPulsePath, output);
            }
        }
    }
}
