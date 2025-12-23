# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files first for better layer caching
COPY ["UrlShortener.Web/UrlShortener.Web.csproj", "UrlShortener.Web/"]
COPY ["UrlShortener.BLL/UrlShortener.BLL.csproj", "UrlShortener.BLL/"]
COPY ["UrlShortener.DAL/UrlShortener.DAL.csproj", "UrlShortener.DAL/"]
COPY ["UrlShortener.Shared/UrlShortener.Shared.csproj", "UrlShortener.Shared/"]
COPY ["UrlShortener.Database/UrlShortener.Database.csproj", "UrlShortener.Database/"]

# Restore dependencies
RUN dotnet restore "UrlShortener.Web/UrlShortener.Web.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/UrlShortener.Web"
RUN dotnet build "UrlShortener.Web.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "UrlShortener.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && \
    apt-get install -y --no-install-recommends curl && \
    rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=publish /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "UrlShortener.Web.dll"]
