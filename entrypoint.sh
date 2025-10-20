#!/bin/bash
set -e

echo "Проверяем миграции..."

# Создаём миграцию, если нет ни одной (например, при первом запуске)
if [ ! -d "./Migrations" ] || [ -z "$(ls -A ./Migrations 2>/dev/null)" ]; then
    echo "Миграции не найдены — создаём InitialCreate..."
    dotnet ef migrations add InitialCreate --output-dir Migrations --project DnDAPI.dll || echo "Не удалось создать миграцию"
else
    echo "Миграции найдены, пропускаем создание."
fi

echo "Применяем миграции к базе..."
dotnet ef database update --project DnDAPI.dll || echo "Не удалось применить миграции (возможно, БД ещё не готова)."

echo "Запускаем API..."
exec dotnet DnDAPI.dll
