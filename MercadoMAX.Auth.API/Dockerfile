FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MercadoMAX.Auth.API/MercadoMAX.Auth.API.csproj", "MercadoMAX.Auth.API/"]
COPY ["MercadoMAX.Shared/MercadoMAX.Shared.csproj", "MercadoMAX.Shared/"]
RUN dotnet restore "MercadoMAX.Auth.API/MercadoMAX.Auth.API.csproj"
COPY . .
WORKDIR "/src/MercadoMAX.Auth.API"
RUN dotnet build "MercadoMAX.Auth.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MercadoMAX.Auth.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MercadoMAX.Auth.API.dll"]
