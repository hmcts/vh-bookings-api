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

##Branch name git hook will run on pre commit and control the standard for new branch name.

The branch name should start with: feature/VIH-XXXX-branchName  (X - is digit).
If git version is less than 2.9 the pre-commit file from the .githooks folder need copy to local .git/hooks folder.
To change git hooks directory to directory under source control run (works only for git version 2.9 or greater) :
$ git config core.hooksPath .githooks

##Commit message 
The commit message will be validated by prepare-commit-msg hook.
The commit message format should start with : 'feature/VIH-XXXX : ' folowing by 8 or more characters description of commit, otherwise the warning message will be presented.

```

## Run Zap scan locally

To run Zap scan locally update the following settings and run acceptance\integration tests

Update following configuration under appsettings.json under VideoApi.AcceptanceTests or  VideoApi.IntegrationTests

- "Services:BookingsApiUrl": "https://BookingsApi_AC/"
- "ZapConfiguration:ZapScan": true
- "ConnectionStrings:VhBookings": "Server=localhost,1433;Database=VhBookings;User=sa;Password=VeryStrongPassword!;" (IntegrationTest alone)

Note: Ensure you have Docker desktop engine installed and setup

## Run Stryker

To run stryker mutation test, go to UnitTest folder under command prompt and run the following command

```bash
dotnet stryker
```

From the results look for line(s) of code highlighted with Survived\No Coverage and fix them.


If in case you have not installed stryker previously, please use one of the following commands

### Global
```bash
dotnet tool install -g dotnet-stryker
```
### Local
```bash
dotnet tool install dotnet-stryker
```

To update latest version of stryker please use the following command

```bash
dotnet tool update --global dotnet-stryker
```