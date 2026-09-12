@echo off
chcp 65001 > nul

cd C:\otus-forum\GatewayService
docker-compose down
docker-compose up --build -d

pause
