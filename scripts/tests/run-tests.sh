#!/bin/sh

echo "Current DIR ${PWD}"
# echo "ProjectKey ${SONAR_PROJECT_KEY}"
# echo "ProjectToken ${SONAR_TOKEN}"
# echo "Restoring dotnet tools"
# dotnet tool install --global dotnet-sonarscanner
# export PATH="$PATH:/root/.dotnet/tools"

# echo "Sonar Begining"
# dotnet sonarscanner begin /k:"${SONAR_PROJECT_KEY}" /d:sonar.cs.opencover.reportsPaths="${PWD}/Coverage/coverage.opencover.xml" /o:hmcts /d:sonar.login="${SONAR_TOKEN}"
# dotnet sonarscanner begin /k:"${SONAR_PROJECT_KEY}" /d:sonar.login="${SONAR_TOKEN}" /d:sonar.cs.opencover.reportsPaths="${PWD}/Coverage/coverage.opencover.xml"

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

# dotnet test BookingsApi/BookingsApi.IntegrationTests/BookingsApi.IntegrationTests.csproj -c Release --no-build --filter FullyQualifiedName~BookingsApi.IntegrationTests.Database --results-directory ./TestResults --logger "trx;LogFileName=BookingsApi-Integration-Tests-TestResults.trx" \
#     "/p:CollectCoverage=true" \
#     "/p:Exclude=\"[*]Testing.Common.*,[*]BookingsApi.Common.*,[*.Common]*\"" \
#     "/p:CoverletOutput=/Coverage/" \
#     "/p:MergeWith={PWD}/Coverage/coverage.json" \
#     "/p:CoverletOutputFormat=\"opencover,json,cobertura,lcov\""

# echo "Sonar End"
# dotnet sonarscanner end /d:sonar.login="${SONAR_TOKEN}"
