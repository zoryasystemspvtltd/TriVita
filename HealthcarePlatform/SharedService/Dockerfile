# docker build -f SharedService/Dockerfile .

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["BuildingBlocks/Healthcare.Common/Healthcare.Common.csproj", "BuildingBlocks/Healthcare.Common/"]
COPY ["SharedService/SharedService.API/SharedService.API.csproj", "SharedService/SharedService.API/"]
COPY ["SharedService/SharedService.Application/SharedService.Application.csproj", "SharedService/SharedService.Application/"]
COPY ["SharedService/SharedService.Domain/SharedService.Domain.csproj", "SharedService/SharedService.Domain/"]
COPY ["SharedService/SharedService.Infrastructure/SharedService.Infrastructure.csproj", "SharedService/SharedService.Infrastructure/"]
COPY ["SharedService/SharedService.Contracts/SharedService.Contracts.csproj", "SharedService/SharedService.Contracts/"]

RUN dotnet restore "SharedService/SharedService.API/SharedService.API.csproj"

COPY BuildingBlocks/ BuildingBlocks/
COPY SharedService/ SharedService/

WORKDIR "/src/SharedService/SharedService.API"
RUN dotnet publish "SharedService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SharedService.API.dll"]
