FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy wait-for-it.sh into our image
ADD https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh /wait-for-it.sh
# Make it executable, in Linux
RUN chmod +x /wait-for-it.sh

COPY ["scripts/tests/run-tests.sh", "/"]
RUN chmod +x /run-tests.sh

# Copy project files to cache layer
COPY ["BookingsApi/nuget.config", "BookingsApi/"]
COPY ["BookingsApi/BookingsApi/BookingsApi.csproj", "BookingsApi/BookingsApi/"]
COPY ["BookingsApi/BookingsApi.Contract/BookingsApi.Contract.csproj", "BookingsApi/BookingsApi.Contract/"]
COPY ["BookingsApi/BookingsApi.Common/BookingsApi.Common.csproj", "BookingsApi/BookingsApi.Common/"]
COPY ["BookingsApi/BookingsApi.DAL/BookingsApi.DAL.csproj", "BookingsApi/BookingsApi.DAL/"]
COPY ["BookingsApi/BookingsApi.Domain/BookingsApi.Domain.csproj", "BookingsApi/BookingsApi.Domain/"]
COPY ["BookingsApi/BookingsApi.Infrastructure.Services/BookingsApi.Infrastructure.Services.csproj", "BookingsApi/BookingsApi.Infrastructure.Services/"]

COPY ["BookingsApi/BookingsApi.UnitTests/BookingsApi.UnitTests.csproj", "BookingsApi/BookingsApi.UnitTests/"]
COPY ["BookingsApi/BookingsApi.IntegrationTests/BookingsApi.IntegrationTests.csproj", "BookingsApi/BookingsApi.IntegrationTests/"]
COPY ["BookingsApi/Testing.Common/Testing.Common.csproj", "BookingsApi/Testing.Common/"]

# Restore project files to cache layer
# RUN sed -i "s|</configuration>|<packageSourceCredentials><MicroserviceDemoNugets><add key=\"Username\" value=\"PAT\" /><add key=\"ClearTextPassword\" value=\"${PAT}\" /></MicroserviceDemoNugets></packageSourceCredentials></configuration>|" BookingsApi/nuget.config


RUN dotnet restore "BookingsApi/BookingsApi.Common/BookingsApi.Common.csproj"
RUN dotnet restore "BookingsApi/BookingsApi.Domain/BookingsApi.Domain.csproj"
RUN dotnet restore "BookingsApi/BookingsApi.DAL/BookingsApi.DAL.csproj"
RUN dotnet restore "BookingsApi/BookingsApi.Contract/BookingsApi.Contract.csproj"
RUN dotnet restore "BookingsApi/BookingsApi/BookingsApi.csproj"
RUN dotnet restore "BookingsApi/BookingsApi.Infrastructure.Services/BookingsApi.Infrastructure.Services.csproj"

RUN dotnet restore "BookingsApi/BookingsApi.UnitTests/BookingsApi.UnitTests.csproj"
RUN dotnet restore "BookingsApi/BookingsApi.IntegrationTests/BookingsApi.IntegrationTests.csproj"

# Add remaining files prone to change
COPY BookingsApi BookingsApi