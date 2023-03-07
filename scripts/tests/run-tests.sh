#!/bin/sh
set -x

rm -d -r ${PWD}/Coverage
rm -d -r ${PWD}/TestResults

# dotnet sonarscanner begin /k:"${SONAR_PROJECT_KEY}" /o:"${SONAR_ORG}" /version:"${SONAR_PROJECT_VERSION}" /name:"${SONAR_PROJECT_NAME}" /d:sonar.host.url="${SONAR_HOST}" /d:sonar.login="${SONAR_TOKEN}" /d:sonar.cs.opencover.reportsPaths="${PWD}/Coverage/coverage.opencover.xml" /d:sonar.coverage.exclusions="**/BookingsApi/Swagger/**/*,**/Program.cs,**/Startup.cs,**/Testing.Common/**/*,**/BookingsApi.Common/**/*,**/BookingsApi.IntegrationTests/**/*,**/BookingsApi.UnitTests/**/*,**/BookingsApi/Extensions/*,**/BookingsApi.DAL/Migrations/**/*" /d:sonar.cpd.exclusions="**/Program.cs,**/Startup.cs,**/Testing.Common/**/*,**/BookingsApi/Swagger/**/*,BookingsApi/BookingsApi.DAL/Migrations/*,BookingsApi/BookingsApi.DAL/TemplateDataForEnvironments.cs"

exclusions="[Testing.Common]*,[BookingsApi.Common]BookingsApi.Common.*,[BookingsApi.Domain]*.Ddd*,[BookingsApi.DAL]*.Migrations*,[BookingsApi]*.Swagger"

dotnet build BookingsApi/BookingsApi.sln -c Release
# Script is for docker compose tests where the script is at the root level
dotnet test BookingsApi/BookingsApi.UnitTests/BookingsApi.UnitTests.csproj -c Release --no-build --results-directory ./TestResults --logger "trx;LogFileName=BookingsApi-Unit-Tests-TestResults.trx" \
    "/p:CollectCoverage=true" \
    "/p:Exclude=\"${exclusions}\"" \
    "/p:CoverletOutput=${PWD}/Coverage/" \
    "/p:MergeWith=${PWD}/Coverage/coverage.json" \
    "/p:CoverletOutputFormat=\"opencover,json,cobertura,lcov\""

dotnet test BookingsApi/BookingsApi.IntegrationTests/BookingsApi.IntegrationTests.csproj -c Release --no-build --results-directory ./TestResults --logger "trx;LogFileName=BookingsApi-Integration-Tests-TestResults.trx" \
    "/p:CollectCoverage=true" \
    "/p:Exclude=\"${exclusions}\"" \
    "/p:CoverletOutput=${PWD}/Coverage/" \
    "/p:MergeWith=${PWD}/Coverage/coverage.json" \
    "/p:CoverletOutputFormat=\"opencover,json,cobertura,lcov\""

# dotnet sonarscanner end /d:sonar.login="${SONAR_TOKEN}"
