# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Directory.Build.props Directory.Packages.props nuget.config global.json Lab.Core.sln ./
COPY src/ src/
COPY tests/ tests/
RUN dotnet restore Lab.Core.sln
RUN dotnet build Lab.Core.sln -c Release --no-restore

FROM build AS test
RUN dotnet test Lab.Core.sln -c Release --no-build --logger "console;verbosity=normal"

FROM build AS pack
ARG VERSION=1.0.0
RUN dotnet pack Lab.Core.sln -c Release --no-restore -p:Version=${VERSION} -o /packages

FROM scratch AS packages
COPY --from=pack /packages /
