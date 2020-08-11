FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

COPY ["Publisher/Publisher.csproj", "src/Publisher/"]
COPY ["Application/Application.csproj", "src/Application/"]
COPY ["Domain/Domain.csproj", "src/Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "src/Publisher/"

COPY . ./
RUN dotnet publish "src/Publisher/" -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Publisher.dll"]