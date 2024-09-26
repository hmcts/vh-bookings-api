# Force the use of user secrets

In the program file add the following line before we loop to add the keyvaults in `ConfigureAppConfiguration`:

``` csharp
configBuilder.AddUserSecrets<Startup>();
```
