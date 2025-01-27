# Setup Dev Container

``` bash
wget -qO- https://aka.ms/install-artifacts-credprovider.sh | bash
dotnet tool restore
dotnet restore --interactive BookingsApi/BookingsApi.sln
```

## TODO

Investigate a quicker and more streamlined solution for nuget config auth or is the credential provider suitable?

Look at turning off authentication and HSTS for Dev environment
