FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG DX_NUGET_SOURCE
WORKDIR /source

# copy csproj and restore as distinct layers.
COPY *.sln .
COPY XAFContainerExample.Blazor.Server/*.csproj ./XAFContainerExample.Blazor.Server/
COPY XAFContainerExample.Module/*.csproj ./XAFContainerExample.Module/
RUN dotnet nuget add source $DX_NUGET_SOURCE -n devexpress-nuget

# The similar issue described here: https://stackoverflow.com/questions/61167032/error-netsdk1064-package-dnsclient-1-2-0-was-not-found
# RUN dotnet restore

COPY XAFContainerExample.Blazor.Server/. ./XAFContainerExample.Blazor.Server/
COPY XAFContainerExample.Module/. ./XAFContainerExample.Module/
WORKDIR /source/XAFContainerExample.Blazor.Server
RUN dotnet publish -c release -o /app --no-cache /restore


FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT [ "dotnet", "XAFContainerExample.Blazor.Server.dll" ]
