# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy .csproj and restore as distinct layers
COPY ["TodoApp.Api.csproj", "."]
RUN dotnet restore "TodoApp.Api.csproj"

# Copy the remaining files and build
COPY . .
RUN dotnet build "TodoApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the build
FROM build AS publish
RUN dotnet publish "TodoApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image setup
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TodoApp.Api.dll"]
