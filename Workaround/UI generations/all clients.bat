@echo off
chcp 65001 > nul

cd /d "%~dp0..\..\UIService\UIService.Clients"
echo Текущая папка: %CD%

# Topics
nswag openapi2csclient /input:http://localhost:5294/openapi/v1.json /output:TopicsClient.g.cs /namespace:OtusForum.UI.Clients.Topics /className:TopicsClient /generateClientInterfaces:true /jsonLibrary:SystemTextJson

# Users
nswag openapi2csclient /input:http://localhost:5225/openapi/v1.json /output:UsersClient.g.cs /namespace:OtusForum.UI.Clients.Users /className:UsersClient /generateClientInterfaces:true /jsonLibrary:SystemTextJson

# Comments
nswag openapi2csclient /input:http://localhost:5044/openapi/v1.json /output:CommentsClient.g.cs /namespace:OtusForum.UI.Clients.Comments /className:CommentsClient /generateClientInterfaces:true /jsonLibrary:SystemTextJson

pause