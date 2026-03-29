# Build from repository root: HealthcarePlatform
# docker build -f LISService/Dockerfile .

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["BuildingBlocks/Healthcare.Common/Healthcare.Common.csproj", "BuildingBlocks/Healthcare.Common/"]
COPY ["LISService/LISService.API/LISService.API.csproj", "LISService/LISService.API/"]
COPY ["LISService/LISService.Application/LISService.Application.csproj", "LISService/LISService.Application/"]
COPY ["LISService/LISService.Domain/LISService.Domain.csproj", "LISService/LISService.Domain/"]
COPY ["LISService/LISService.Infrastructure/LISService.Infrastructure.csproj", "LISService/LISService.Infrastructure/"]
COPY ["LISService/LISService.Contracts/LISService.Contracts.csproj", "LISService/LISService.Contracts/"]

RUN dotnet restore "LISService/LISService.API/LISService.API.csproj"

COPY BuildingBlocks/ BuildingBlocks/
COPY LISService/ LISService/

WORKDIR "/src/LISService/LISService.API"
RUN dotnet publish "LISService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LISService.API.dll"]
