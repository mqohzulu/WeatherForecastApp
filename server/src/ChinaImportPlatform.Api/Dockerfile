# Multi-stage build: SDK image to build/publish, ASP.NET runtime image to run.
# Build context is the `server/` directory.

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ChinaImportPlatform.sln ./
COPY src/ChinaImportPlatform.Api/ChinaImportPlatform.Api.csproj src/ChinaImportPlatform.Api/
RUN dotnet restore src/ChinaImportPlatform.Api/ChinaImportPlatform.Api.csproj

COPY . .
RUN dotnet publish src/ChinaImportPlatform.Api/ChinaImportPlatform.Api.csproj \
    -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ChinaImportPlatform.Api.dll"]
