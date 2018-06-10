microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY BeatPulse.sln ./
COPY Directory.Build.props ./
COPY ./build/dependencies.props ./build/
COPY ./docker-images/BeatPulseUI-Image/BeatPulseUI-Image.csproj docker-images/BeatPulseUI-Image/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/docker-images/BeatPulseUI-Image
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BeatPulseUI-Image.dll"]
