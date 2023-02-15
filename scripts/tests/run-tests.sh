#!/bin/sh

echo "Current DIR ${PWD}"
echo "Restoring dotnet tools"
dotnet tool restore

echo "Sonar Begining"
dotnet sonarscanner begin /k:"${SONAR_PROJECTKEY}" /d:sonar.cs.opencover.reportsPaths="${PWD}/Coverage/coverage.opencover.xml" /o:hmcts

echo "Building solution"
dotnet build BookingsApi/BookingsApi.sln -c Release

echo "Running tests"
# Script is for docker compose tests where the script is at the root level
dotnet test BookingsApi/BookingsApi.UnitTests/BookingsApi.UnitTests.csproj -c Release --no-build --results-directory ./TestResults --logger "trx;LogFileName=BookingsApi-Unit-Tests-TestResults.trx" \
    "/p:CollectCoverage=true" \
    "/p:Exclude=\"[*]Testing.Common.*,[*]BookingsApi.Common.*,[*.Common]*\"" \
    "/p:CoverletOutput=${PWD}/Coverage/" \
    "/p:MergeWith=/Coverage/coverage.json" \
    "/p:CoverletOutputFormat=\"opencover,json,cobertura,lcov\""

dotnet test BookingsApi/BookingsApi.IntegrationTests/BookingsApi.IntegrationTests.csproj -c Release --no-build --filter FullyQualifiedName~BookingsApi.IntegrationTests.Database --results-directory ./TestResults --logger "trx;LogFileName=BookingsApi-Integration-Tests-TestResults.trx" \
    "/p:CollectCoverage=true" \
    "/p:Exclude=\"[*]Testing.Common.*,[*]BookingsApi.Common.*,[*.Common]*\"" \
    "/p:CoverletOutput=/Coverage/" \
    "/p:MergeWith={PWD}/Coverage/coverage.json" \
    "/p:CoverletOutputFormat=\"opencover,json,cobertura,lcov\""

echo "Sonar End"
dotnet sonarscanner end
