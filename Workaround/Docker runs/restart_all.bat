@echo off
start "" "restart_comments.bat"
timeout /t 2

start "" "restart_topics.bat"
timeout /t 2

start "" "restart_users.bat"
timeout /t 2

start "" "restart_gateway.bat"
timeout /t 2

start "" "restart_ui.bat"
timeout /t 2
