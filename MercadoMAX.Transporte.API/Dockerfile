FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MercadoMAX.Transporte.API/MercadoMAX.Transporte.API.csproj", "MercadoMAX.Transporte.API/"]
COPY ["MercadoMAX.Shared/MercadoMAX.Shared.csproj", "MercadoMAX.Shared/"]
RUN dotnet restore "MercadoMAX.Transporte.API/MercadoMAX.Transporte.API.csproj"
COPY . .
WORKDIR "/src/MercadoMAX.Transporte.API"
RUN dotnet build "MercadoMAX.Transporte.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MercadoMAX.Transporte.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MercadoMAX.Transporte.API.dll"]
