@echo off
chcp 65001 > nul

cd /d "%~dp0..\..\CommentsService"
echo Текущая папка: %CD%
docker-compose down
docker-compose up --build -d

pause
