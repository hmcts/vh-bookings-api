# Create Base Image.
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Publish .NET App.
FROM mcr.microsoft.com/dotnet/core/sdk:6.0 AS publish
WORKDIR /src
COPY BookingsApi .
RUN dotnet restore --configfile nuget.sds.config "BookingsApi/BookingsApi.csproj"
WORKDIR "/src/BookingsApi"
RUN dotnet publish "BookingsApi.csproj" -c Release -o /app/publish

# Create Final Image.
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookingsApi.dll"]
