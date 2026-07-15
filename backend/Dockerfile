# === Step 1: Main image ===
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# === Step 2: Build image ===
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src
ARG BUILD_CONFIGURATION=Release

COPY ["./Modules.Authentication/Modules.Authentication.csproj", "./Modules.Authentication/"]
COPY ["./API/API.csproj", "./API/"]

RUN dotnet restore "./Modules.Authentication/Modules.Authentication.csproj"
RUN dotnet restore "./API/API.csproj"

COPY . .

WORKDIR "/src/API"
RUN dotnet build "API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# === Step 3: Publish ===
FROM build AS publish
RUN dotnet publish "API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# === Step 4: Final minimal image ===
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER $APP_UID

ENTRYPOINT ["dotnet", "API.dll"]