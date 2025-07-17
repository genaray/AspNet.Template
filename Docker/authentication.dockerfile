#############
# Base Image
#############
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

USER app
WORKDIR /app

EXPOSE 8080
EXPOSE 8081

#############
# Build Svelte
#############
FROM node:20 AS build-frontend

WORKDIR /src/Frontend
COPY ["AuthenticationService/Frontend/package.json", "AuthenticationService/Frontend/package-lock.json*", "./"]

RUN npm ci

COPY ["AuthenticationService/Frontend/", "./"]
RUN npm run build

################
# Build ASP.Net
################
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-backend
ARG BUILD_CONFIGURATION=Debug

WORKDIR /src
COPY ["AuthenticationService/AuthenticationService.csproj", "./"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore

# Copy other files
COPY AuthenticationService/ AuthenticationService/
COPY Shared/ Shared/

# Copy Frontend build into /wwwroot
RUN mkdir -p AuthenticationService/wwwroot # Ensure folder exists
COPY --from=build-frontend /src/Frontend/dist AuthenticationService/wwwroot/
RUN dotnet build "AuthenticationService/AuthenticationService.csproj" -c $BUILD_CONFIGURATION -o /app/build  # Build! 

################
# Publish
################
FROM build-backend AS publish

ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "AuthenticationService/AuthenticationService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

################
# Finalize
################
FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "AuthenticationService.dll"]