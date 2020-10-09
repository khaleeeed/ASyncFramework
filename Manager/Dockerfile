# command for build image 
# create image   :  docker build -t publisher -f publisher/dockerfile .
# create container :  docker run -d -p 8080:80 publisher publisher

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app
 
COPY ["Manager/Manager.csproj", "src/Manager/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "src/Manager/"
 
COPY . ./
RUN dotnet publish "Manager/" -c Release -o out
 
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Manager.dll"]