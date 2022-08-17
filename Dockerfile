FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG DX_NUGET_SOURCE
WORKDIR /source

# copy csproj and restore as distinct layers.
COPY *.sln .
COPY LoadTestingApp.Blazor.Server/*.csproj ./LoadTestingApp.Blazor.Server/
COPY LoadTestingApp.Module/*.csproj ./LoadTestingApp.Module/
RUN dotnet nuget add source $DX_NUGET_SOURCE -n devexpress-nuget

# The similar issue described here: https://stackoverflow.com/questions/61167032/error-netsdk1064-package-dnsclient-1-2-0-was-not-found
# RUN dotnet restore

COPY LoadTestingApp.Blazor.Server/. ./LoadTestingApp.Blazor.Server/
COPY LoadTestingApp.Module/. ./LoadTestingApp.Module/
WORKDIR /source/LoadTestingApp.Blazor.Server
RUN dotnet publish -c release -o /app --no-cache /restore


FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT [ "dotnet", "LoadTestingApp.Blazor.Server.dll" ]
