FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Bookstore.API/Bookstore.API.csproj", "src/Bookstore.API/"]
COPY ["src/Bookstore.Application/Bookstore.Application.csproj", "src/Bookstore.Application/"]
COPY ["src/Bookstore.Domain/Bookstore.Domain.csproj", "src/Bookstore.Domain/"]
COPY ["src/Bookstore.Infrastructure/Bookstore.Infrastructure.csproj", "src/Bookstore.Infrastructure/"]
RUN dotnet restore "src/Bookstore.API/Bookstore.API.csproj"
COPY . .
WORKDIR "/src/src/Bookstore.API"
RUN dotnet build "Bookstore.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bookstore.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bookstore.API.dll"]
