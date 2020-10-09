# command for build image 
# create image   :  docker build -t publisher -f publisher/dockerfile .
# create container :  docker run -d -p 8080:80 publisher publisher

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app
 
COPY ["Subscriber/Subscriber.csproj", "src/Subscriber/"]
COPY ["Application/Application.csproj", "src/Application/"]
COPY ["Domain/Domain.csproj", "src/Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "src/Subscriber/"
 
COPY . ./
RUN dotnet publish "Subscriber/" -c Release -o out
 
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Subscriber.dll"]