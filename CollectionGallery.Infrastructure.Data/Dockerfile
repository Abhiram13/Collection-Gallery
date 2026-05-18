ARG NUGET_PAT
ARG DOTNET_VERSION=10.0-alpine

# Build Base Image with SDK
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS base
ARG NUGET_PAT

# Create and Set working directory inside docker container
WORKDIR /collection-gallery

# Run restore command inside container
RUN dotnet nuget add source --name "github" "https://nuget.pkg.github.com/Abhiram13/index.json" --username Abhiram13 --password "${NUGET_PAT}" --store-password-in-clear-text

FROM base AS restore
COPY CollectionGallery.sln ./
COPY Directory.Build.props ./
COPY Directory.Packages.props ./
COPY CollectionGallery.Infrastructure.Data/ ./CollectionGallery.Infrastructure.Data/
COPY CollectionGallery.Shared/ ./CollectionGallery.Shared/
RUN dotnet restore CollectionGallery.Infrastructure.Data/CollectionGallery.Infrastructure.Data.csproj

FROM restore AS dev
WORKDIR /collection-gallery/CollectionGallery.Infrastructure.Data
CMD ["dotnet", "watch", "run"]

FROM restore AS build
RUN dotnet build CollectionGallery.Infrastructure.Data/CollectionGallery.Infrastructure.Data.csproj -c Release --no-restore

FROM build AS publish
RUN dotnet publish CollectionGallery.Infrastructure.Data/CollectionGallery.Infrastructure.Data.csproj -c Release -o /out --no-build

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS runtime
WORKDIR /collection-gallery
COPY --from=publish /out ./
ENTRYPOINT [ "dotnet", "CollectionGallery.Infrastructure.Data.dll" ]