# Use the official .NET 9.0 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# Use the official .NET 9.0 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["CarlosAOliveira.Developer.Worker/CarlosAOliveira.Developer.Worker.csproj", "CarlosAOliveira.Developer.Worker/"]
COPY ["CarlosAOliveira.Developer.Application/CarlosAOliveira.Developer.Application.csproj", "CarlosAOliveira.Developer.Application/"]
COPY ["CarlosAOliveira.Developer.Domain/CarlosAOliveira.Developer.Domain.csproj", "CarlosAOliveira.Developer.Domain/"]
COPY ["CarlosAOliveira.Developer.ORM/CarlosAOliveira.Developer.ORM.csproj", "CarlosAOliveira.Developer.ORM/"]
COPY ["CarlosAOliveira.Developer.Common/CarlosAOliveira.Developer.Common.csproj", "CarlosAOliveira.Developer.Common/"]

# Restore dependencies
RUN dotnet restore "CarlosAOliveira.Developer.Worker/CarlosAOliveira.Developer.Worker.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/CarlosAOliveira.Developer.Worker"
RUN dotnet build "CarlosAOliveira.Developer.Worker.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "CarlosAOliveira.Developer.Worker.csproj" -c Release -o /app/publish

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

# Start the application
ENTRYPOINT ["dotnet", "CarlosAOliveira.Developer.Worker.dll"]
