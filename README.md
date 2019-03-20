# vh-bookings-api


## Running code coverage

First ensure you are running a terminal in the BookingsAPI directory of this repository and then run the following commands.

``` bash
dotnet test Bookings.UnitTests/Bookings.UnitTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat="\"opencover,cobertura,json,lcov\"" /p:CoverletOutput=../Artifacts/Coverage/ /p:MergeWith='../Artifacts/Coverage/coverage.json' /p:Exclude="\"[*]Bookings.Common.*,[*]Testing.Common.*,[Bookings.DAL]Bookings.DAL.BookingsDbContext,[*]Bookings.DAL.Mappings,[*]Bookings.DAL.Migrations,[*]Bookings.DAL.SeedData.*,[*]Bookings.DAL.Exceptions.*,[*]Bookings.DAL.Mappings.*,[*]Bookings.DAL.Migrations.*,[*]Bookings.Domain.Ddd.*,[Bookings.DAL]Bookings.DAL.DesignTimeHearingsContextFactory,[*]Bookings.Domain.Validations.*,[Bookings.API]Bookings.API.ConfigureServicesExtensions,[*]Bookings.API.Extensions.*,[*]Bookings.API.Swagger.*,[Bookings.API]Bookings.API.Program,[Bookings.API]Bookings.API.Startup\""

dotnet test Bookings.IntegrationTests/Bookings.IntegrationTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat="\"opencover,cobertura,json,lcov\"" /p:CoverletOutput=../Artifacts/Coverage/ /p:MergeWith='../Artifacts/Coverage/coverage.json' /p:Exclude="\"[*]Bookings.Common.*,[*]Testing.Common.*,[Bookings.DAL]Bookings.DAL.BookingsDbContext,[*]Bookings.DAL.Mappings,[*]Bookings.DAL.Migrations,[*]Bookings.DAL.SeedData.*,[*]Bookings.DAL.Exceptions.*,[*]Bookings.DAL.Mappings.*,[*]Bookings.DAL.Migrations.*,[*]Bookings.Domain.Ddd.*,[Bookings.DAL]Bookings.DAL.DesignTimeHearingsContextFactory,[*]Bookings.Domain.Validations.*,[Bookings.API]Bookings.API.ConfigureServicesExtensions,[*]Bookings.API.Extensions.*,[*]Bookings.API.Swagger.*,[Bookings.API]Bookings.API.Program,[Bookings.API]Bookings.API.Startup\""

```

## Running Sonar Analysis

``` bash
dotnet sonarscanner begin /k:"vh-bookings-api" /d:sonar.cs.opencover.reportsPaths="BookingsAPI/Artifacts/Coverage/coverage.opencover.xml" /d:sonar.coverage.exclusions="Bookings.API/Program.cs,Bookings.API/Startup.cs,Bookings.API/Extensions/**,Bookings.API/Swagger/**,Bookings.API/ConfigureServicesExtensions.cs,Testing.Common/**,Bookings.Common/**,Bookings.DAL/Mappings/**,Bookings.DAL/SeedData/**,Bookings.DAL/BookingsDbContext.cs,Bookings.DAL/**/DesignTimeHearingsContextFactory.cs,Bookings.DAL/Migrations/**,Bookings.Domain/Ddd/**,Bookings.Domain/Validations/**,Bookings.DAL/Commands/Core/**,Bookings.DAL/Queries/Core/**" /d:sonar.cpd.exclusions="Bookings.DAL/Migrations/**" /d:sonar.verbose=true
dotnet build BookingsAPI/BookingsApi.sln
dotnet sonarscanner end

```


## Generate HTML Report

In the BookingsAPI directory after having run either or both test projects:

``` bash
dotnet reportgenerator "-reports:Artifacts/Coverage/coverage.opencover.xml" "-targetDir:Artifacts/Coverage/Report" -reporttypes:HtmlInline_AzurePipelines
```