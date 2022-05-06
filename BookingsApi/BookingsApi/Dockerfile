#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM gittools/gitversion:5.6.10-alpine.3.12-x64-3.1 AS gitversion
#WORKDIR /version 
#COPY .git .git
#RUN /tools/dotnet-gitversion /ensureassemblyinfo /updateassemblyinfo AssemblyInfo.cs /nofetch /config /GitVersion.yml

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY BookingsApi .
RUN dotnet restore --configfile nuget.config "BookingsApi/BookingsApi.csproj"
WORKDIR "/src/BookingsApi"
COPY --from=gitversion /version/AssemblyInfo.cs .
RUN dotnet build "BookingsApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BookingsApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookingsApi.dll"]