FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-alpine AS build
WORKDIR /src
COPY ./Toss.Client ./Toss.Client
COPY ./Toss.Server ./Toss.Server
COPY ./Toss.Shared ./Toss.Shared
RUN dotnet restore Toss.Server/Toss.Server.csproj
RUN dotnet publish Toss.Server/Toss.Server.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Toss.Server.dll"]
