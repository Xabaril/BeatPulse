using BeatPulse.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Owin
{
    public class OwinBeatPulseService : IBeatPulseService
    {
        private readonly BeatPulseContext _beatPulseContext;
        public OwinBeatPulseService(BeatPulseContext beatPulseContext)
        {
            _beatPulseContext = beatPulseContext;
        }

        public async Task<IEnumerable<LivenessResult>> IsHealthy(string path, BeatPulseOptions beatPulseOptions)
        {
            var livenessResults = new List<LivenessResult>();
            foreach (var registration in _beatPulseContext.GetAllLiveness(path))
            {
                var liveness = _beatPulseContext.CreateLivenessFromRegistration(registration);
                var livenessContext = LivenessExecutionContext
                    .FromRegistration(registration, true);
                var livenessResult = await RunLiveness(
                    liveness,
                    livenessContext,
                    beatPulseOptions
                   );
                livenessResults.Add(livenessResult);
                if (!livenessResult.IsHealthy && !beatPulseOptions.DetailedOutput)
                {
                    //if is unhealthy and not detailed options is true return inmediatly

                    //_logger.LogWarning($"Liveness {livenessContext.Name} is not healthy. Breaking liveness execution because detailed output is false.");

                    return livenessResults;
                }
            }
            return livenessResults;
        }

        async Task<LivenessResult> RunLiveness(IBeatPulseLiveness liveness, LivenessExecutionContext livenessContext, BeatPulseOptions beatPulseOptions)
        {
            //_logger.LogInformation($"Executing liveness {livenessContext.Name}.");

            var livenessResult = new LivenessResult(livenessContext.Name, livenessContext.Path);

            livenessResult.StartCounter();

            try
            {
                using (var cancellationTokenSource = new CancellationTokenSource())
                {
                    var livenessTask = liveness.IsHealthy(livenessContext, cancellationTokenSource.Token);

                    if (await Task.WhenAny(livenessTask, Task.Delay(beatPulseOptions.Timeout, cancellationTokenSource.Token)) == livenessTask)
                    {
                        // The liveness is executed successfully and get the results
                        var (message, healthy) = await livenessTask;
                        livenessResult.StopCounter(message, healthy);
                        // _logger.LogInformation($"The liveness {livenessContext.Name} is executed.");
                    }
                    else
                    {
                        // The liveness is timeout ( from configured options)
                        //_logger.LogWarning($"The liveness {livenessContext.Name} is timeout, execution is cancelled.");
                        cancellationTokenSource.Cancel();
                        livenessResult.StopCounter(BeatPulseKeys.BEATPULSE_TIMEOUT_MESSAGE, false);
                    }
                }
            }
            catch (Exception ex)
            {
                // The uri executed is now well formed, dns not found
                // or any unexpected errror 

                //_logger.LogError(ex, $"The liveness {livenessContext.Name} is unhealthy.");

                var message = true
                    ? ex.Message : string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, livenessContext.Name);

                livenessResult.StopCounter(message, false);
            }

            return livenessResult;
        }
    }
}
