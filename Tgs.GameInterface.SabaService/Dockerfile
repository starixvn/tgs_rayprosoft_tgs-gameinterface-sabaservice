#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BuildConfiguration=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameInterface.SabaService/Tgs.GameInterface.SabaService.csproj", "Tgs.GameInterface.SabaService/"]
COPY ["Tgs.GameInterface.SabaService.Core/Tgs.GameInterface.SabaService.Core.csproj", "Tgs.GameInterface.SabaService.Core/"]
RUN dotnet restore "Tgs.GameInterface.SabaService/Tgs.GameInterface.SabaService.csproj"
COPY . .
WORKDIR "/src/Tgs.GameInterface.SabaService"
RUN dotnet build "Tgs.GameInterface.SabaService.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.GameInterface.SabaService.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameInterface.SabaService.dll"]