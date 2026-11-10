@echo off
echo ========================================
echo   Генератор тестовых данных форума
echo ========================================
echo.

echo [1/3] AuthUsersService...
docker exec -i auth-users-postgres psql -U postgres -d auth-users < auth-users.sql
if %errorlevel% neq 0 (
    echo Ошибка при выполнении auth-users.bat!
    pause
    exit /b 1
)

echo.
echo [2/3] TopicsService...
docker exec -i topics-postgres psql -U postgres -d topics-api < topics-api.sql
if %errorlevel% neq 0 (
    echo Ошибка при выполнении topics-api.bat!
    pause
    exit /b 1
)

echo.
echo [3/3] CommentsService...
docker exec -i comments-postgres psql -U postgres -d comments-api < comments-api.sql
if %errorlevel% neq 0 (
    echo Ошибка при выполнении comments-api.bat!
    pause
    exit /b 1
)

echo.
echo ========================================
echo   Готово!
echo ========================================
pause
