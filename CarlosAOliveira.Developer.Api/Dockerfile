# Use the official .NET 9.0 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official .NET 9.0 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["CarlosAOliveira.Developer.Api/CarlosAOliveira.Developer.Api.csproj", "CarlosAOliveira.Developer.Api/"]
COPY ["CarlosAOliveira.Developer.Application/CarlosAOliveira.Developer.Application.csproj", "CarlosAOliveira.Developer.Application/"]
COPY ["CarlosAOliveira.Developer.Domain/CarlosAOliveira.Developer.Domain.csproj", "CarlosAOliveira.Developer.Domain/"]
COPY ["CarlosAOliveira.Developer.ORM/CarlosAOliveira.Developer.ORM.csproj", "CarlosAOliveira.Developer.ORM/"]
COPY ["CarlosAOliveira.Developer.Common/CarlosAOliveira.Developer.Common.csproj", "CarlosAOliveira.Developer.Common/"]

# Restore dependencies
RUN dotnet restore "CarlosAOliveira.Developer.Api/CarlosAOliveira.Developer.Api.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/CarlosAOliveira.Developer.Api"
RUN dotnet build "CarlosAOliveira.Developer.Api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "CarlosAOliveira.Developer.Api.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

# Create runtime directory for SQLite
RUN mkdir -p /app/runtime

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80;https://+:443
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=""
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=""

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
  CMD curl -f http://localhost/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "CarlosAOliveira.Developer.Api.dll"]
