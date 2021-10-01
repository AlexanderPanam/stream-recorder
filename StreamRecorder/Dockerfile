FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY . .
RUN ls
RUN dotnet restore "StreamRecorder.csproj"
RUN dotnet build "StreamRecorder.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StreamRecorder.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN apt update
RUN apt install streamlink -y
RUN mkdir recordings
ENTRYPOINT ["dotnet", "StreamRecorder.dll"]