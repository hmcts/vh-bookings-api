#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["BookingApi/BookingApi.csproj", "BookingApi/"]
COPY ["BookingsApi.Common/Bookings.Common.csproj", "BookingsApi.Common/"]
COPY ["BookingsApi.DAL/BookingsApi.DAL.csproj", "BookingsApi.DAL/"]
COPY ["BookingsApi.Domain/BookingsApi.Domain.csproj", "BookingsApi.Domain/"]
COPY ["BookingsApi.Infrastructure.Services/BookingsApi.Infrastructure.Services.csproj", "BookingsApi.Infrastructure.Services/"]
COPY ["BookingsApi.Contract/BookingsApi.Contract.csproj", "BookingsApi.Contract/"]
RUN dotnet restore "BookingsApi/BookingsApi.csproj"
COPY . .
WORKDIR "/src/BookingsApi"
RUN dotnet build "BookingsApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BookingsApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookingsApi.dll"]