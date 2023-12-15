FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN apt-get update
RUN apt-get install -y libc6 libicu-dev libfontconfig1
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
RUN --mount=type=secret,id=dxnuget dotnet nuget add source $(cat /run/secrets/dxnuget) -n devexpress-nuget
COPY ["XAFContainerExample.Blazor.Server/XAFContainerExample.Blazor.Server.csproj", "XAFContainerExample.Blazor.Server/"]
COPY ["XAFContainerExample.Module/XAFContainerExample.Module.csproj", "XAFContainerExample.Module/"]
RUN dotnet restore "XAFContainerExample.Blazor.Server/XAFContainerExample.Blazor.Server.csproj"
COPY . .
WORKDIR "/src/XAFContainerExample.Blazor.Server"
RUN dotnet build "XAFContainerExample.Blazor.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "XAFContainerExample.Blazor.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "XAFContainerExample.Blazor.Server.dll"]
