FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build

ARG BUILDCONFIG=RELEASE
ARG VERSION=1.0.0

WORKDIR /app

# Copy .sln and .csproj files and restore nugets, later we can cache this layer

# Copy solution file into /app
COPY ./Locker.sln  ./

# Copy the main source project files 
COPY src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done

# copy src folder into /app/src
COPY src/. ./src/

WORKDIR /app/src/Locker.Api

# build Locker.Api project and output into /app/dist
RUN dotnet publish -c Release -o ../../dist

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime

WORKDIR /app/dist
# Copy /app/dist from previous image into /app/dist of current 
COPY --from=build /app/dist ./

ENTRYPOINT ["dotnet", "Locker.Api.dll"] 
