

# Szintaxis kijavítása és a megfelelő verziók használata
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Csak a csproj fájlok másolása és helyreállítása
COPY SolarWatch.csproj ./
RUN dotnet restore

# Az összes fájl másolása és build
COPY . ./
RUN dotnet publish -c Release -o out  \
#lefordítja a projektet a Release konfigurációval az out mappába helyezi el
#-o out /app/out a cél mappa

# Runtime image építése
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "SolarWatch.dll"]


