# vh-bookings-api

## Running code coverage

First ensure you are running a terminal in the BookingsAPI directory of this repository and then run the following commands.

``` bash
dotnet test --no-build Bookings.UnitTests/Bookings.UnitTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat="\"opencover,cobertura,json,lcov\"" /p:CoverletOutput=../Artifacts/Coverage/ /p:MergeWith='../Artifacts/Coverage/coverage.json' /p:Exclude="\"[*]Bookings.Common.*,[*]Testing.Common.*,[Bookings.DAL]Bookings.DAL.BookingsDbContext,[*]Bookings.DAL.Mappings,[*]Bookings.DAL.Migrations,[*]Bookings.DAL.SeedData.*,[*]Bookings.DAL.Exceptions.*,[*]Bookings.DAL.Mappings.*,[*]Bookings.DAL.Migrations.*,[*]Bookings.DAL.Commands.Core.*,[*]Bookings.DAL.Queries.Core.*,[*]Bookings.Domain.Ddd.*,[Bookings.DAL]Bookings.DAL.DesignTimeHearingsContextFactory,[*]Bookings.Domain.Validations.*,[Bookings.API]Bookings.API.ConfigureServicesExtensions,[*]Bookings.API.Extensions.*,[*]Bookings.API.Swagger.*,[Bookings.API]Bookings.API.Program,[Bookings.API]Bookings.API.Startup\""

dotnet test --no-build Bookings.IntegrationTests/Bookings.IntegrationTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat="\"opencover,cobertura,json,lcov\"" /p:CoverletOutput=../Artifacts/Coverage/ /p:MergeWith='../Artifacts/Coverage/coverage.json' /p:Exclude="\"[*]Bookings.Common.*,[*]Testing.Common.*,[Bookings.DAL]Bookings.DAL.BookingsDbContext,[*]Bookings.DAL.Mappings,[*]Bookings.DAL.Migrations,[*]Bookings.DAL.SeedData.*,[*]Bookings.DAL.Exceptions.*,[*]Bookings.DAL.Mappings.*,[*]Bookings.DAL.Migrations.*,[*]Bookings.DAL.Commands.Core.*,[*]Bookings.DAL.Queries.Core.*,[*]Bookings.Domain.Ddd.*,[Bookings.DAL]Bookings.DAL.DesignTimeHearingsContextFactory,[*]Bookings.Domain.Validations.*,[Bookings.API]Bookings.API.ConfigureServicesExtensions,[*]Bookings.API.Extensions.*,[*]Bookings.API.Swagger.*,[Bookings.API]Bookings.API.Program,[Bookings.API]Bookings.API.Startup\""

```

## Generate HTML Report

Under the unit test project directory

``` bash
dotnet reportgenerator "-reports:../Artifacts/Coverage/coverage.opencover.xml" "-targetDir:../Artifacts/Coverage/Report" -reporttypes:HtmlSummary
```