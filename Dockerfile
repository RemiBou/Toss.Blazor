FROM microsoft/dotnet:3.0-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80


FROM remibou/blazor-build-08 AS build
WORKDIR /src
COPY . .
WORKDIR /src/Toss.Server
RUN dotnet restore -nowarn:msb3202,nu1503
RUN dotnet build --no-restore -c Release -o /app


FROM build as unittest
WORKDIR /src/Toss.Tests

FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Toss.Server.dll"]