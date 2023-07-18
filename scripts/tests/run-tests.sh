#!/bin/sh
set -x

exclusions="[Testing.Common]*,[BookingsApi.Common]BookingsApi.Common.*,[BookingsApi.Domain]*.Ddd*,[BookingsApi.DAL]*.Migrations*,[BookingsApi]*.Swagger"
configuration=Release

# Script is for docker compose tests where the script is at the root level
dotnet test BookingsApi/BookingsApi.UnitTests/BookingsApi.UnitTests.csproj -c $configuration --results-directory ./TestResults --logger "trx;LogFileName=BookingsApi-Unit-Tests-TestResults.trx" \
    "/p:CollectCoverage=true" \
    "/p:Exclude=\"${exclusions}\"" \
    "/p:CoverletOutput=${PWD}/Coverage/" \
    "/p:MergeWith=${PWD}/Coverage/coverage.json" \
    "/p:CoverletOutputFormat=\"opencover,json,cobertura,lcov\"" ||
    {
        echo "##vso[task.logissue type=error]DotNet Unit Tests Failed."
        echo "##vso[task.complete result=Failed]"
        exit 1
    }

dotnet tool update dotnet-ef --global
dotnet ef database update -p BookingsApi/BookingsApi.DAL/BookingsApi.DAL.csproj -s BookingsApi/BookingsApi.DAL/BookingsApi.DAL.csproj
dotnet ef database update -p RefData/RefData.csproj -s RefData/RefData.csproj

dotnet test BookingsApi/BookingsApi.IntegrationTests/BookingsApi.IntegrationTests.csproj -c $configuration --results-directory ./TestResults --logger "trx;LogFileName=BookingsApi-Integration-Tests-TestResults.trx" \
    "/p:CollectCoverage=true" \
    "/p:Exclude=\"${exclusions}\"" \
    "/p:CoverletOutput=${PWD}/Coverage/" \
    "/p:MergeWith=${PWD}/Coverage/coverage.json" \
    "/p:CoverletOutputFormat=\"opencover,json,cobertura,lcov\"" ||
    {
        echo "##vso[task.logissue type=error]DotNet Integration Tests Failed."
        echo "##vso[task.complete result=Failed]"
        exit 1
    }
