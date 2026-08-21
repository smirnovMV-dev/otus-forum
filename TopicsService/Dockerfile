FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .

RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish "TopicsService.API/TopicsService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5294
ENV ASPNETCORE_URLS=http://+:5294
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "TopicsService.API.dll"]
