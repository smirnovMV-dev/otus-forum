@echo off
chcp 65001 > nul

cd C:\otus-forum\CommentsService
docker-compose down
docker-compose up --build -d

pause
