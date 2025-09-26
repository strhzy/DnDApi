#!/bin/bash
set -e

echo "⏳ Ждём базу данных..."
# Можно добавить healthcheck/wait-for-it, если надо ждать postgres/mysql
# пример для postgres:
# until pg_isready -h db -p 5432 -U $POSTGRES_USER; do
#   sleep 2
# done

echo "📦 Применяем миграции..."
dotnet ef database update --no-build --project DnDAPI.csproj

echo "🚀 Запускаем приложение..."
exec dotnet DnDAPI.dll
