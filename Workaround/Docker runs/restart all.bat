@echo off
chcp 65001 > nul

cd /d "%~dp0..\..\CommentsService"
echo Текущая папка: %CD%
docker-compose down
docker-compose up --build -d

timeout /t 2

cd /d "%~dp0..\..\TopicsService"
echo Текущая папка: %CD%
docker-compose down
docker-compose up --build -d

timeout /t 2

cd /d "%~dp0..\..\AuthUsersService"
echo Текущая папка: %CD%
docker-compose down
docker-compose up --build -d

timeout /t 2

cd /d "%~dp0..\..\GatewayService"
echo Текущая папка: %CD%
docker-compose down
docker-compose up --build -d

timeout /t 2

cd /d "%~dp0..\..\UIService"
echo Текущая папка: %CD%
docker-compose down
docker-compose up --build -d

pause
