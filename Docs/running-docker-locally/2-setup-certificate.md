# Create a new dev certificate

``` bash
dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp-vh.pfx -p password
dotnet dev-certs https --trust
```
