FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 as build-env
WORKDIR /
COPY GabenDiscordBot.sln ./
# copy csproj and restore as distinct layers
COPY GabenBot.Domain/*.csproj ./GabenBot.Domain/
COPY GabenDiscordBot/*.csproj ./GabenDiscordBot/
RUN dotnet restore

# copy everything else and build app
COPY . .
WORKDIR /GabenBot.Domain
RUN dotnet build -c Release -o /app

WORKDIR /GabenDiscordBot
RUN dotnet build -c Release -o /app

FROM build-env AS publish
RUN dotnet publish -c Release -o /app

# final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "GabenDiscordBot.dll"]
