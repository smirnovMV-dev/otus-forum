@echo off
chcp 65001 > nul

cd C:\otus-forum\UIService
docker-compose down
docker-compose up --build -d

pause
