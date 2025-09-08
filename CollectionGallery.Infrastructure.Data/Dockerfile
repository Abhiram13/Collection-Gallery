# Use a build argument for the PAT
ARG NUGET_PAT

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy solution and props from root (build context must be repo root)
COPY CollectionGallery.sln ./
COPY Directory.Packages.props ./

# Copy the projects individually
COPY CollectionGallery.Infrastructure.Data/ ./CollectionGallery.Infrastructure.Data/
COPY CollectionGallery.Domain.Models/ ./CollectionGallery.Domain.Models/
COPY CollectionGallery.Shared/ ./CollectionGallery.Shared/

# Re-declare the build argument to make it available in this stage
ARG NUGET_PAT

# Add the private package source using the build argument
RUN dotnet nuget add source --name "Github" "https://nuget.pkg.github.com/Abhiram13/index.json" --username Abhiram13 --password "${NUGET_PAT}" --store-password-in-clear-text

# Copy csproj and restore as distinct layers
# COPY *.csproj ./
# RUN dotnet restore "Storage.csproj"

# Restore and publish only Storage.csproj
RUN dotnet restore CollectionGallery.Infrastructure.Data/CollectionGallery.Infrastructure.Data.csproj
RUN dotnet publish CollectionGallery.Infrastructure.Data/CollectionGallery.Infrastructure.Data.csproj -c Release -o /out

# Copy everything else and build
# COPY . .
# RUN dotnet publish "Storage.csproj" -c Release -o /out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /out ./

ENTRYPOINT [ "dotnet", "CollectionGallery.Infrastructure.Data.dll" ]