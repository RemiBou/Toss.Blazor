FROM microsoft/dotnet:3.0-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM remibou/blazor-build-08 AS build
WORKDIR /src
COPY Toss.Server Toss.Server/
COPY Toss.Client Toss.Client/
COPY Toss.Shared Toss.Shared/
RUN dotnet restore Toss.Server/Toss.Server.csproj

WORKDIR /src/Toss.Server
RUN dotnet publish Toss.Server.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Toss.Server.dll"]
