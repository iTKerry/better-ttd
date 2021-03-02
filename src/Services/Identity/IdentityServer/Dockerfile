#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src

COPY ["src/Services/Identity/IdentityServer/IdentityServer.csproj", "src/Services/Identity/IdentityServer/"]
COPY ["src/Services/Identity/IdentityServer.DataAccess.EF/IdentityServer.DataAccess.EF.csproj", "src/Services/Identity/IdentityServer.DataAccess.EF/"]

RUN dotnet restore "src/Services/Identity/IdentityServer/IdentityServer.csproj"
RUN dotnet restore "src/Services/Identity/IdentityServer.DataAccess.EF/IdentityServer.DataAccess.EF.csproj"

COPY . .

RUN dotnet build "/src/src/Services/Identity/IdentityServer/IdentityServer.csproj" -c Release -o /app/build
RUN dotnet build "/src/src/Services/Identity/IdentityServer.DataAccess.EF/IdentityServer.DataAccess.EF.csproj" -c Release -o /app/build

WORKDIR "/src/src/Services/Identity/IdentityServer"

FROM build AS publish
RUN dotnet publish "IdentityServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .fsproj
ENTRYPOINT ["dotnet", "IdentityServer.dll"]
