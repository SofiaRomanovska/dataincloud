FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/CatalogCloud.API/CatalogCloud.API.csproj", "src/CatalogCloud.API/"]
COPY ["src/CatalogCloud.Application/CatalogCloud.Application.csproj", "src/CatalogCloud.Application/"]
COPY ["src/CatalogCloud.Domain/CatalogCloud.Domain.csproj", "src/CatalogCloud.Domain/"]
COPY ["src/CatalogCloud.Infrastructure/CatalogCloud.Infrastructure.csproj", "src/CatalogCloud.Infrastructure/"]
RUN dotnet restore "src/CatalogCloud.API/CatalogCloud.API.csproj"
COPY . .
WORKDIR "/src/src/CatalogCloud.API"
RUN dotnet build "CatalogCloud.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CatalogCloud.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CatalogCloud.API.dll"]
