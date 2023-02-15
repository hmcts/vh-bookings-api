FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy wait-for-it.sh into our image
ADD https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh /wait-for-it.sh
# Make it executable, in Linux
RUN chmod +x /wait-for-it.sh

COPY ["scripts/tests/run-tests.sh", "/"]
RUN chmod +x /run-tests.sh

# Add remaining files prone to change
COPY BookingsApi BookingsApi