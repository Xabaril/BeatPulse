# Version 1.7.2 to 2.0

    1. Refactor *BeatPulseOptions* public surface.
    2. ConfigureDetailedOutput method on *BeatPulseOptions* support flag configuration.
    3. **UseBeatPulse** as *IApplicationBuilder* extension method to support include existing middlewares on *BeatPulse* requests.
    4. Use *CamelCaseContractResolver* on *BeatPulse* serialization json responses.
    5. BeatPulse support executing multiple liveness with the same path.
    6. New registration options that give us more control over the liveness configuration.
    7. Remove UseCors on BeatPulseOptions, related with -> 3.

