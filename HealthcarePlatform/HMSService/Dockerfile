# Build from repository root: HealthcarePlatform
# docker build -f HMSService/Dockerfile .

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["BuildingBlocks/Healthcare.Common/Healthcare.Common.csproj", "BuildingBlocks/Healthcare.Common/"]
COPY ["HMSService/HMSService.API/HMSService.API.csproj", "HMSService/HMSService.API/"]
COPY ["HMSService/HMSService.Application/HMSService.Application.csproj", "HMSService/HMSService.Application/"]
COPY ["HMSService/HMSService.Domain/HMSService.Domain.csproj", "HMSService/HMSService.Domain/"]
COPY ["HMSService/HMSService.Infrastructure/HMSService.Infrastructure.csproj", "HMSService/HMSService.Infrastructure/"]
COPY ["HMSService/HMSService.Contracts/HMSService.Contracts.csproj", "HMSService/HMSService.Contracts/"]

RUN dotnet restore "HMSService/HMSService.API/HMSService.API.csproj"

COPY BuildingBlocks/ BuildingBlocks/
COPY HMSService/ HMSService/

WORKDIR "/src/HMSService/HMSService.API"
RUN dotnet publish "HMSService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HMSService.API.dll"]
