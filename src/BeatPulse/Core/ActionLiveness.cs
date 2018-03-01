using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Core
{
    public class ActionLiveness
        : IBeatPulseLiveness
    {
        private readonly Func<HttpContext, CancellationToken,Task<(string, bool)>> _check;
        private readonly string _name;
        private readonly string _defaultPath;

        public string Name => _name;

        public string DefaultPath => _defaultPath;

        public ActionLiveness(string name, string defaultPath,Func<HttpContext,CancellationToken,Task<(string, bool)>> check)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _defaultPath = defaultPath ?? throw new ArgumentNullException(nameof(_defaultPath));
            _check = check ?? throw new ArgumentNullException(nameof(check));
        }

        public Task<(string, bool)> IsHealthy(HttpContext context,bool isDevelopment,CancellationToken cancellationToken = default)
        {
            return _check(context,cancellationToken);
        }
    }
}
