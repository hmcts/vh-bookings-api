#!/bin/sh

# Script is for docker compose tests where the script is at the root level
dotnet test BookingsApi/BookingsApi.UnitTests/BookingsApi.UnitTests.csproj -c Release --no-build --results-directory ./TestResults --logger "trx;LogFileName=BookingsApi-Unit-Tests-TestResults.trx" \
    "/p:CollectCoverage=true" \
    "/p:Exclude=\"[*]Testing.Common.*,[*]BookingsApi.Common.*,[*.Common]*\"" \
    "/p:CoverletOutput=/Coverage/" \
    "/p:MergeWith=/Coverage/coverage.json" \
    "/p:CoverletOutputFormat=\"json,cobertura\""

dotnet test BookingsApi/BookingsApi.IntegrationTests/BookingsApi.IntegrationTests.csproj -c Release --no-build --filter FullyQualifiedName~BookingsApi.IntegrationTests.Database --results-directory ./TestResults --logger "trx;LogFileName=BookingsApi-Integration-Tests-TestResults.trx" \
    "/p:CollectCoverage=true" \
    "/p:Exclude=\"[*]Testing.Common.*,[*]BookingsApi.Common.*,[*.Common]*\"" \
    "/p:CoverletOutput=/Coverage/" \
    "/p:MergeWith=/Coverage/coverage.json" \
    "/p:CoverletOutputFormat=\"json,cobertura\""
