# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy project file
COPY ["BroadYap/BroadYap.csproj", "BroadYap/"]

# Restore dependencies
RUN dotnet restore "BroadYap/BroadYap.csproj"

# Copy all source code
COPY . .

# Build the application
RUN dotnet build "BroadYap/BroadYap.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "BroadYap/BroadYap.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD dotnet --version || exit 1

ENTRYPOINT ["dotnet", "BroadYap.dll"]