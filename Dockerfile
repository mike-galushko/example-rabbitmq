#######################
# Publish API service #
#######################

# 1. Take a docker image suitable for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# 2. Go to a folder inside the Build container
WORKDIR /App
# 3. Copy sources to the /App folder
# Syntax: <src> <dst>
# src in the build context
# dst inside the docker image
COPY ./ ./

RUN ls

# 4. Restore dependencies
RUN dotnet restore
# 5. Build and publish
RUN dotnet publish -o out

###############
# Build image #
###############

# 1. Take a docker image suitable to run the service
FROM mcr.microsoft.com/dotnet/aspnet:8.0
# 2. Go a folder inside the Run container
WORKDIR /App
# 3. Copy files from Build container to the Run container
COPY --from=build /App/out .
# 4. Specify to use dotnet on container run
ENTRYPOINT ["dotnet", "RabbitMQ.WebAPI.dll"]
