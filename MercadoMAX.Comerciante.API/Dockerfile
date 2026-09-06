FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MercadoMAX.Comerciante.API/MercadoMAX.Comerciante.API.csproj", "MercadoMAX.Comerciante.API/"]
COPY ["MercadoMAX.Shared/MercadoMAX.Shared.csproj", "MercadoMAX.Shared/"]
RUN dotnet restore "MercadoMAX.Comerciante.API/MercadoMAX.Comerciante.API.csproj"
COPY . .
WORKDIR "/src/MercadoMAX.Comerciante.API"
RUN dotnet build "MercadoMAX.Comerciante.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MercadoMAX.Comerciante.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MercadoMAX.Comerciante.API.dll"]
