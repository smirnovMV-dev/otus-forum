@echo off
echo === AuthUsersService Seed ===
echo Container: auth-users-postgres
echo Database: auth-users
echo Port: 5533
echo.

docker exec -i auth-users-postgres psql -U postgres -d auth-users < auth-users.sql

if %errorlevel% equ 0 (
    echo.
    echo Готово!
) else (
    echo.
    echo Ошибка выполнения!
    pause
)
