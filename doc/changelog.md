# Change Log

## UrlGroup 2.1.0
    1. Added library for check url groups liveness

## SqlLite 2.1.1
    1. Fix issue with AddSqlLite extension method namespace, now is no BeatPulse.

## DocumentDb 2.1.1
     1. Fix issue with AddDocumentDb extension method namespace, now is no BeatPulse.

## Version 1.7.2 to 2.1

    1. ASP.NET Core 2.1 dependency
    2. Refactor *BeatPulseOptions* public surface.
    3. ConfigureDetailedOutput method on *BeatPulseOptions* support flag configuration.
    4. **UseBeatPulse** as *IApplicationBuilder* extension method to support include existing middlewares on *BeatPulse* requests.
    6. Use *CamelCaseContractResolver* on *BeatPulse* serialization json responses.
    7. BeatPulse support executing multiple liveness with the same path.
    8. New registration options that give us more control over the liveness configuration.
    9. Remove UseCors on BeatPulseOptions, related with -> 3.
    10. Improved BeatPulseUI.