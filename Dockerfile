FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["hopefullyAWebForum.csproj", "./"]
RUN dotnet restore "hopefullyAWebForum.csproj"

COPY . .
RUN dotnet build "hopefullyAWebForum.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "hopefullyAWebForum.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "hopefullyAWebForum.dll"]
