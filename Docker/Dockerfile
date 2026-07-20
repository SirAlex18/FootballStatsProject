# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies first to leverage Docker cache
COPY ["IntegrationToApi.csproj", "./"]
RUN dotnet restore

# Copy source code and publish
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
<<<<<<< HEAD
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Install curl for healthcheck
=======
ENV ASPNETCORE_URLS=http://+:8080

# Install curl for healthcheck (compose uses it)
>>>>>>> 62a3223d093aec1e543a62eb25394438af07e1b4
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published output from build stage
COPY --from=build /app/publish .

<<<<<<< HEAD
EXPOSE 80

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:80/ || exit 1

=======
EXPOSE 8080
>>>>>>> 62a3223d093aec1e543a62eb25394438af07e1b4
ENTRYPOINT ["dotnet", "IntegrationToApi.dll"]
