@echo off
chcp 65001 > nul

cd C:\otus-forum\AuthUsersService
docker-compose down
docker-compose up --build -d

pause
