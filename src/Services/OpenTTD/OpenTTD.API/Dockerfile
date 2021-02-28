#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/Services/OpenTTD/OpenTTD.API/OpenTTD.API.fsproj", "src/Services/OpenTTD/OpenTTD.API/"]
COPY ["src/Services/OpenTTD/OpenTTD.Network/OpenTTD.Network.fsproj", "src/Services/OpenTTD/OpenTTD.Network/"]
RUN dotnet restore "src/Services/OpenTTD/OpenTTD.API/OpenTTD.API.fsproj"
COPY . .
WORKDIR "/src/src/Services/OpenTTD/OpenTTD.API"
RUN dotnet build "OpenTTD.API.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenTTD.API.fsproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenTTD.API.dll"]
