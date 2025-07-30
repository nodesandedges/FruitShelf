FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FruitShelf.API/FruitShelf.API.csproj", \
      "FruitShelf.Data/FruitShelf.Data.csproj", \
      "FruitShelf.API.Tests/FruitShelf.API.Tests.csproj", \
      "FruitShelf.Data.Tests/FruitShelf.Data.Tests.csproj", \
      "FruitShelf.API/"]
      
RUN dotnet restore "FruitShelf.API/FruitShelf.API.csproj"
COPY . .
WORKDIR "/src/FruitShelf.API"
RUN dotnet build "./FruitShelf.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# TEST STAGE (runs before publish)
FROM build AS test
WORKDIR /src/FruitShelf.Tests
RUN dotnet test "./FruitShelf.Tests.csproj" -c $BUILD_CONFIGURATION --no-build --logger trx

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FruitShelf.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FruitShelf.API.dll"]
