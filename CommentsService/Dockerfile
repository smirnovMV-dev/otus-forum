FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .

RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish "CommentsService.API/CommentsService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5044
EXPOSE 5045

ENV ASPNETCORE_KESTREL__ENDPOINTS__HTTP1__URL=http://+:5044
ENV ASPNETCORE_KESTREL__ENDPOINTS__HTTP1__PROTOCOLS=Http1

ENV ASPNETCORE_KESTREL__ENDPOINTS__HTTP2__URL=http://+:5045
ENV ASPNETCORE_KESTREL__ENDPOINTS__HTTP2__PROTOCOLS=Http2

ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "CommentsService.API.dll"]
