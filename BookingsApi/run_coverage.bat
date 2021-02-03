rmdir /q /s Artifacts

SET exclude=\"**/Bookings.API/Program.cs, **/Bookings.API/Startup.cs, **/Bookings.API/Extensions/**, **/Bookings.API/Swagger/**, **/Bookings.API/ConfigureServicesExtensions.cs, **/Testing.Common/**, **/Bookings.Common/**, **/Bookings.DAL/Mappings/**, **/Bookings.DAL/SeedData/**, **/Bookings.DAL/BookingsDbContext.cs, **/Bookings.DAL/**/DesignTimeHearingsContextFactory.cs, Bookings.DAL/Migrations/**, **/Bookings.Domain/Ddd/**, **/Bookings.DAL/Commands/Core/**, **/Bookings.DAL/Queries/Core/**, **/Testing.Common/**, **/Bookings.DAL/Migrations/*, **/Bookings.DAL/Migrations/**, **/Migrations/*\"
dotnet test Bookings.UnitTests/Bookings.UnitTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat="\"opencover,cobertura,json,lcov\"" /p:CoverletOutput=../Artifacts/Coverage/ /p:MergeWith='../Artifacts/Coverage/coverage.json' /p:Exclude="%exclude%"
dotnet test Bookings.IntegrationTests/Bookings.IntegrationTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat="\"opencover,cobertura,json,lcov\"" /p:CoverletOutput=../Artifacts/Coverage/ /p:MergeWith='../Artifacts/Coverage/coverage.json' /p:Exclude="%exclude%"

reportgenerator -reports:Artifacts/Coverage/coverage.opencover.xml -targetDir:Artifacts/Coverage/Report -reporttypes:HtmlInline_AzurePipelines

"Artifacts/Coverage/Report/index.htm"