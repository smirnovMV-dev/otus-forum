@echo off
echo === CommentsService Seed ===
echo Container: comments-postgres
echo Database: comments-api
echo Port: 5434
echo.

docker exec -i comments-postgres psql -U postgres -d comments-api < comments-api.sql

if %errorlevel% equ 0 (
    echo.
    echo Готово!
) else (
    echo.
    echo Ошибка выполнения!
    pause
)
