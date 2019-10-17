FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-alpine AS build
# Install .NET Core for ravendb embbeded code taken from https://github.com/dotnet/dotnet-docker/blob/master/2.2/runtime/alpine3.9/amd64/Dockerfile
ENV DOTNET_VERSION 2.2.7

RUN wget -O dotnet.tar.gz https://dotnetcli.blob.core.windows.net/dotnet/Runtime/$DOTNET_VERSION/dotnet-runtime-$DOTNET_VERSION-linux-musl-x64.tar.gz \
    && dotnet_sha512='e5e5437b57041395bf0af0b7472a615fb6aaef72a052a9b16d891cf0e9ea8a9cb09c28e0bf1d0f4808d5b61648324f3ab77dddcc7e426ba1fb91235a039eaaf1' \
    && echo "$dotnet_sha512  dotnet.tar.gz" | sha512sum -c - \    
    && tar -C /usr/share/dotnet -xzf dotnet.tar.gz \
    && rm dotnet.tar.gz

WORKDIR /src
COPY ./Toss.Client/Toss.Client.csproj ./Toss.Client/
COPY ./Toss.Server/Toss.Server.csproj ./Toss.Server/
COPY ./Toss.Shared/Toss.Shared.csproj ./Toss.Shared/
COPY ./Toss.Tests/Toss.Tests.csproj ./Toss.Tests/
COPY ./Toss.sln ./
RUN dotnet restore ./Toss.sln
COPY ./Toss.Client ./Toss.Client
COPY ./Toss.Server ./Toss.Server
COPY ./Toss.Shared ./Toss.Shared
COPY ./Toss.Tests ./Toss.Tests
RUN dotnet test ./Toss.Tests
RUN dotnet publish Toss.Server/Toss.Server.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Toss.Server.dll"]
