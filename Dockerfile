# Базовый образ для runtime (используем SDK для поддержки dotnet ef)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS base
USER $APP_UID
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

# Установка dotnet-ef и создание миграций
RUN dotnet tool install --global dotnet-ef --version 9.0.0
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet ef migrations add InitialCreate --output-dir Migrations --project DnDAPI.csproj

WORKDIR "/src/"
RUN dotnet build "./DnDAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этап публикации
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DnDAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src/Migrations ./Migrations/
COPY --from=build /src/Templates ./Templates/
RUN ls -la /app/Templates/
RUN ls -la /app/Migrations/

# Применение миграций при запуске контейнера, затем запуск приложения
ENTRYPOINT ["dotnet", "DnDAPI.dll"]