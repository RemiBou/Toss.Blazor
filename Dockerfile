
FROM mcr.microsoft.com/dotnet/core/runtime:2.2.7-alpine as runtime227
FROM mcr.microsoft.com/dotnet/core/sdk:3.1.201-alpine AS build
# import sdk from 2.2.7 because we need it for running ravendb embedded
COPY --from=runtime227 /usr/share/dotnet /usr/share/dotnet 
WORKDIR /src
COPY ./Toss.Client/Toss.Client.csproj ./Toss.Client/
COPY ./Toss.Server/Toss.Server.csproj ./Toss.Server/
COPY ./Toss.Shared/Toss.Shared.csproj ./Toss.Shared/
COPY ./Toss.Tests/Toss.Tests.csproj ./Toss.Tests/
COPY ./Toss.sln ./
RUN dotnet restore ./Toss.sln
COPY ./Toss.Shared ./Toss.Shared
COPY ./Toss.Server ./Toss.Server
COPY ./Toss.Tests ./Toss.Tests
COPY ./Toss.Client ./Toss.Client
RUN dotnet test ./Toss.Tests
RUN dotnet publish Toss.Server/Toss.Server.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /app .
EXPOSE 80
ENTRYPOINT ["dotnet", "Toss.Server.dll"]
