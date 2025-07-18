#############
# Base Image
#############
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

USER app
WORKDIR /app

EXPOSE 8080
EXPOSE 8081

################
# Build ASP.Net
################
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-backend
ARG BUILD_CONFIGURATION=Debug

WORKDIR /src
COPY ["UserService/UserService.csproj", "./"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore
COPY . .

# Copy Frontend build into Gen.Backend/wwwroot
RUN dotnet build "UserService/UserService.csproj" -c $BUILD_CONFIGURATION -o /app/build  # Build! 

################
# Publish
################
FROM build-backend AS publish

ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "UserService/UserService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

################
# Finalize
################
FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserService.dll"]