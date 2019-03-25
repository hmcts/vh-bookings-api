# vh-bookings-api


## Running code coverage

1. Install the report generator dotnet tool
https://www.nuget.org/packages/dotnet-reportgenerator-globaltool/

You may need to restart your prompt to get the updated path.

2. CD into the BookingsAPI folder

3. Run the command for windows or osx `./run_coverage.sh` or `run_coverage.bat`

The coverage report will open automatically after run, joining the results for both integration and unit tests.

## Running Sonar Analysis

``` bash
dotnet sonarscanner begin /k:"vh-bookings-api" /d:sonar.cs.opencover.reportsPaths="BookingsAPI/Artifacts/Coverage/coverage.opencover.xml" /d:sonar.coverage.exclusions="Bookings.API/Program.cs,Bookings.API/Startup.cs,Bookings.API/Extensions/**,Bookings.API/Swagger/**,Bookings.API/ConfigureServicesExtensions.cs,Testing.Common/**,Bookings.Common/**,Bookings.DAL/Mappings/**,Bookings.DAL/SeedData/**,Bookings.DAL/BookingsDbContext.cs,Bookings.DAL/**/DesignTimeHearingsContextFactory.cs,Bookings.DAL/Migrations/**,Bookings.Domain/Ddd/**,Bookings.Domain/Validations/**,Bookings.DAL/Commands/Core/**,Bookings.DAL/Queries/Core/**" /d:sonar.cpd.exclusions="Bookings.DAL/Migrations/**" /d:sonar.verbose=true
dotnet build BookingsAPI/BookingsApi.sln
dotnet sonarscanner end

```