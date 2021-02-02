#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Bookings.API/Bookings.API.csproj", "Bookings.API/"]
COPY ["Bookings.Common/Bookings.Common.csproj", "Bookings.Common/"]
COPY ["Bookings.DAL/Bookings.DAL.csproj", "Bookings.DAL/"]
COPY ["Bookings.Domain/Bookings.Domain.csproj", "Bookings.Domain/"]
COPY ["Bookings.Infrastructure.Services/Bookings.Infrastructure.Services.csproj", "Bookings.Infrastructure.Services/"]
COPY ["Bookings.Api.Contract/Bookings.Api.Contract.csproj", "Bookings.Api.Contract/"]
COPY ["nuget.config", "nuget.config"]
RUN dotnet restore --configfile nuget.config "Bookings.API/Bookings.API.csproj"
COPY . .
WORKDIR "/src/Bookings.API"
RUN dotnet build "Bookings.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bookings.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bookings.API.dll"]