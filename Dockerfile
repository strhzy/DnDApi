# Используем SDK, чтобы иметь доступ к dotnet ef внутри контейнера
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем проект и зависимости
COPY . .
RUN dotnet restore "DnDAPI.csproj"
RUN dotnet publish "DnDAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный образ (SDK, а не runtime — нужен ef)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS final
WORKDIR /app

# Копируем артефакты и скрипт
COPY --from=build /app/publish .
COPY entrypoint.sh .

# Устанавливаем dotnet-ef
RUN dotnet tool install --global dotnet-ef --version 9.0.0
ENV PATH="${PATH}:/root/.dotnet/tools"

# Делаем скрипт исполняемым
RUN chmod +x entrypoint.sh

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["./entrypoint.sh"]
