@echo off
echo === TopicsService Seed ===
echo Container: topics-postgres
echo Database: topics-api
echo Port: 5433
echo.

docker exec -i topics-postgres psql -U postgres -d topics-api < topics-api.sql

if %errorlevel% equ 0 (
    echo.
    echo Готово!
) else (
    echo.
    echo Ошибка выполнения!
    pause
)
