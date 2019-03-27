rm -rf Artifacts

exclude=\"[*]Bookings.Common.*,[*]Testing.Common.*,[Bookings.DAL]Bookings.DAL.BookingsDbContext,[*]Bookings.DAL.Mappings,[*]Bookings.DAL.Migrations,[*]Bookings.DAL.SeedData.*,[*]Bookings.DAL.Exceptions.*,[*]Bookings.DAL.Mappings.*,[*]Bookings.DAL.Migrations.*,[*]Bookings.DAL.Commands.Core.*,[*]Bookings.DAL.Queries.Core.*,[*]Bookings.Domain.Ddd.*,[Bookings.DAL]Bookings.DAL.DesignTimeHearingsContextFactory,[Bookings.API]Bookings.API.ConfigureServicesExtensions,[*]Bookings.API.Extensions.*,[*]Bookings.API.Swagger.*,[Bookings.API]Bookings.API.Program,[Bookings.API]Bookings.API.Startup\"
dotnet test Bookings.UnitTests/Bookings.UnitTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat="\"opencover,cobertura,json,lcov\"" /p:CoverletOutput=../Artifacts/Coverage/ /p:MergeWith='../Artifacts/Coverage/coverage.json' /p:Exclude="${exclude}"
dotnet test Bookings.IntegrationTests/Bookings.IntegrationTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat="\"opencover,cobertura,json,lcov\"" /p:CoverletOutput=../Artifacts/Coverage/ /p:MergeWith='../Artifacts/Coverage/coverage.json' /p:Exclude="${exclude}"

~/.dotnet/tools/reportgenerator -reports:Artifacts/Coverage/coverage.opencover.xml -targetDir:Artifacts/Coverage/Report -reporttypes:HtmlInline_AzurePipelines

open Artifacts/Coverage/Report/index.htm