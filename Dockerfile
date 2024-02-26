FROM alpine:latest
RUN apk update && apk upgrade && apk add --no-cache bash git openssh

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# or use chiseled
# FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled-extra AS base

WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release

WORKDIR /
RUN git clone https://github.com/eMukator/GLA.git

WORKDIR /GLA/src

RUN dotnet restore "LogParser.csproj"
RUN dotnet build "LogParser.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "LogParser.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /GLA/src/log-data /log-data

ENTRYPOINT ["dotnet", "LogParser.dll"]
