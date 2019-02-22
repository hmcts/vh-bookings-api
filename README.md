# vh-bookings-api

## Running code coverage

``` bash
dotnet test --no-build Bookings.UnitTests/Bookings.UnitTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat="\"opencover,cobertura,json,lcov\"" /p:CoverletOutput=../Artifacts/Coverage/ /p:MergeWith='../Artifacts/Coverage/coverage.json' /p:Exclude="\"[Bookings.*Tests?]*,[Bookings.API]Startup,[*]Bookings.DAL.SeedData.*,[*]Bookings.DAL.Migrations.*,[*]Bookings.DAL.Mappings.*,[Bookings.DAL]Bookings.DAL.BookingsDbContext,[Bookings.DAL]Bookings.DAL.DesignTimeHearingsContextFactory,[Bookings.Common]*,[Testing.Common]*\""

dotnet test --no-build Bookings.IntegrationTests/Bookings.IntegrationTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat="\"opencover,cobertura,json,lcov\"" /p:CoverletOutput=../Artifacts/Coverage/ /p:MergeWith='../Artifacts/Coverage/coverage.json' /p:Exclude="\"[Bookings.*Tests?]*,[Bookings.API]Startup,[*]Bookings.DAL.SeedData.*,[*]Bookings.DAL.Migrations.*,[*]Bookings.DAL.Mappings.*,[Bookings.DAL]Bookings.DAL.BookingsDbContext,[Bookings.DAL]Bookings.DAL.DesignTimeHearingsContextFactory,[Bookings.Common]*,[Testing.Common]*\""

```