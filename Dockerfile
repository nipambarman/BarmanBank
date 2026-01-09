# ============================
# Build stage
# ============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /app/publish

# ============================
# Runtime stage
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Docker container port
EXPOSE 8080

# Force ASP.NET to use port 8080 in container
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "helloWorld.dll"]
