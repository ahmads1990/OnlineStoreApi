
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OnlineStoreApi/OnlineStoreApi.csproj", "OnlineStoreApi/"]
RUN dotnet restore "./OnlineStoreApi/./OnlineStoreApi.csproj"
COPY . .
WORKDIR "/src/OnlineStoreApi"
RUN dotnet build "./OnlineStoreApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OnlineStoreApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OnlineStoreApi.dll"]