# Changes to docker compose file

If you wish to run docker using the existing dotnet secrets, edit the docker compose override file and omit the `mnt/secrets` volume.

`- ~/mnt/secrets:/mnt/secrets:ro`

If you wish to run docker using the mounted key per file approach, edit the docker compose override file and omit the dotnet secrets volume.

`- ~/.microsoft/usersecrets/:/root/.microsoft/usersecrets:ro`

``` bash
docker compose -f "docker-compose.override.yml" up -d --build 
```
