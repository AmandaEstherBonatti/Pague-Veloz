# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Install curl for health checks
RUN apt-get update && apt-get install -y curl wget && rm -rf /var/lib/apt/lists/*

# Copy csproj files and restore dependencies
COPY ["PagueVeloz.csproj", "./"]
COPY ["PagueVeloz.Domain/PagueVeloz.Domain.csproj", "PagueVeloz.Domain/"]
COPY ["PagueVeloz.Application/PagueVeloz.Application.csproj", "PagueVeloz.Application/"]
COPY ["PagueVeloz.Infrastructure/PagueVeloz.Infrastructure.csproj", "PagueVeloz.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "PagueVeloz.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src"

# Build arguments
ARG BUILD_CONFIGURATION=Release
RUN dotnet build "PagueVeloz.csproj" -c ${BUILD_CONFIGURATION} --no-restore

# Stage 2: Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PagueVeloz.csproj" -c ${BUILD_CONFIGURATION} -o /app/publish /p:UseAppHost=false --no-build

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Install curl and wget for health checks
RUN apt-get update && apt-get install -y curl wget && rm -rf /var/lib/apt/lists/*

# Create non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published app
COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/logs && chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Optimize for containers
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Run the app
ENTRYPOINT ["dotnet", "PagueVeloz.dll"]
