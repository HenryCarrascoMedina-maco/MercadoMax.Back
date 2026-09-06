FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MercadoMAX.Guias.API/MercadoMAX.Guias.API.csproj", "MercadoMAX.Guias.API/"]
COPY ["MercadoMAX.Shared/MercadoMAX.Shared.csproj", "MercadoMAX.Shared/"]
RUN dotnet restore "MercadoMAX.Guias.API/MercadoMAX.Guias.API.csproj"
COPY . .
WORKDIR "/src/MercadoMAX.Guias.API"
RUN dotnet build "MercadoMAX.Guias.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MercadoMAX.Guias.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MercadoMAX.Guias.API.dll"]
