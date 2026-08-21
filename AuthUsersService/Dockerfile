FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .

RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish "AuthUsersService.API/AuthUsersService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5225
EXPOSE 5226

ENV ASPNETCORE_KESTREL__ENDPOINTS__HTTP1__URL=http://+:5225
ENV ASPNETCORE_KESTREL__ENDPOINTS__HTTP1__PROTOCOLS=Http1

ENV ASPNETCORE_KESTREL__ENDPOINTS__HTTP2__URL=http://+:5226
ENV ASPNETCORE_KESTREL__ENDPOINTS__HTTP2__PROTOCOLS=Http2

ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "AuthUsersService.API.dll"]
