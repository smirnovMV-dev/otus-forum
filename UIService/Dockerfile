FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .

RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish "UIService/UIService/UIService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5129
ENV ASPNETCORE_URLS=http://+:5129
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "UIService.dll"]
