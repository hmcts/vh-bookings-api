# vh-bookings-api

[![Build Status](https://dev.azure.com/hmcts/Video%20Hearings/_apis/build/status/vh-bookings-api/hmcts.vh-bookings-api.sds.master-release?repoName=hmcts%2Fvh-bookings-api&branchName=master)](https://dev.azure.com/hmcts/Video%20Hearings/_build/latest?definitionId=664&repoName=hmcts%2Fvh-bookings-api&branchName=master)

[![BookingsApi.Client package in vh-packages feed in Azure Artifacts](https://feeds.dev.azure.com/hmcts/cf3711aa-2aed-4f62-81a8-2afaee0ce26d/_apis/public/Packaging/Feeds/vh-packages/Packages/900b1d86-dad9-4013-b8ff-cc19a79e7605/Badge)](https://dev.azure.com/hmcts/Video%20Hearings/_artifacts/feed/vh-packages/NuGet/BookingsApi.Client?preferRelease=true)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=vh-bookings-api&metric=alert_status)](https://sonarcloud.io/dashboard?id=vh-bookings-api)

## Restore Tools

Run the following in a terminal at the root of the repository

``` shell
dotnet tool restore
```

## Branch name git hook will run on pre commit and control the standard for new branch name.

The branch name should start with: feature/VIH-XXXX-branchName  (X - is digit).
If git version is less than 2.9 the pre-commit file from the .githooks folder need copy to local .git/hooks folder.
To change git hooks directory to directory under source control run (works only for git version 2.9 or greater) :

`$ git config core.hooksPath .githooks`

## Commit message 

The commit message will be validated by prepare-commit-msg hook.
The commit message format should start with : 'feature/VIH-XXXX : ' folowing by 8 or more characters description of commit, otherwise the warning message will be presented.

## Running all tests in Docker

Open a terminal at the root level of the repository and run the following command

```console
docker-compose -f "docker-compose.tests.yml" up --build --abort-on-container-exit
```

> You may need to create a `.env` file to store the environment variables

## Convert test results into coverage report

Run the following in a terminal

``` bash
dotnet reportgenerator "-reports:./Coverage/coverage.opencover.xml" "-targetDir:./Artifacts/Coverage/Report" -reporttypes:Html -sourcedirs:./BookingsApi
```

## Running the app as a container

Visit the VH-Setup repository for
[Instructions to run as a container locally.](https://github.com/hmcts/vh-setup/tree/main/docs/local-container-setup).
