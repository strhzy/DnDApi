# Базовый образ runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DnDAPI.csproj", "./"]
RUN dotnet restore "DnDAPI.csproj"
COPY . .
RUN dotnet build "DnDAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Публикация
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DnDAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ./entrypoint.sh .
RUN chmod +x entrypoint.sh

ENTRYPOINT ["./entrypoint.sh"]