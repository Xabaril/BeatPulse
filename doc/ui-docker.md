# BeatPulse UI Docker Image

*BeatPulseUI* is available as a docker image on [DockerHub](https://hub.docker.com/r/xabarilcoding/beatpulseui/). This image is a simple ASP.NET Core project with the *BeatPulseUI* middleware.

```bash
docker pull xabarilcoding/beatpulseui 
docker run --name ui -p 5000:80 -d beatpulseui:latest
```

You can use environment variables to configure all properties on *BeatPulseUI*. 

```bash
docker run --name ui -p 5000:80 -e 'BeatPulse-UI:Liveness:0:Name=httpBasic' -e 'BeatPulse-UI:Liveness:0:Uri=http://the-livenes-server-path' -d beatpulseui:latest
```

Read the [DockerHub full description](https://hub.docker.com/r/xabarilcoding/beatpulseui/) to get more information about BeatPulse docker configuration.