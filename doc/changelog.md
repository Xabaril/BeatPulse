# Change Log

## Network 2.1.1

    1. Added IMAP connection liveness, supporting SSL/TLS and STARTTLS handshake.

    - The imap liveness can try account logins and check mailbox
    existance.

    2. Added SMTP connection liveness, supporting SSL/TLS

    - The smtp liveness cant try account logins.

    No further dependencies were added to the package for added livenesses.

## Network 2.1.0
    The new package BeatPulse.Network was added, it includes the following livenesses:

    1. Added Tcp connection liveness
    2. Added Ftp connection liveness
    3. Added Sftp connection liveness
    4. Added Dns resolve liveness
    
## System 2.2.2
    1. Ping liveness was moved to new BeatPulse.Network package as it suites better there (breaking change)

## System 2.2.0
    1. Added private memory process liveness.
    2. Added virtual memory process liveness.
    3. Added working set process liveness.

## System 2.1.0
    1. Added library with system health checks (ping and disk storage)

## Oracle 2.1.0-beta3
    1. Added Oracle beta library to check database health. Currently working with Oracle managed data access library beta 3 version

## UrlGroup 2.1.0
    1. Added library to check url groups liveness

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