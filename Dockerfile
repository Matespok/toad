# 1. Fáze: Sestavení aplikace (zde použijeme velké SDK)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Zkopírujeme projekt a obnovíme závislosti (zrychluje další buildy)
COPY ["toad.csproj", "./"]
RUN dotnet restore "toad.csproj"

# Zkopírujeme zbytek kódů a zkompilujeme v Release režimu
COPY . .
RUN dotnet publish "toad.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 2. Fáze: Spuštění (zde použijeme malý a rychlý ASP.NET Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Produkční nastavení portů (ASP.NET 8 a 9 standardně poslouchá na 8080)
EXPOSE 8080
ENTRYPOINT ["dotnet", "toad.dll"]
