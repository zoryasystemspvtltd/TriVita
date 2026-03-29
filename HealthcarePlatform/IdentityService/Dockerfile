# docker build -f IdentityService/Dockerfile .

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["BuildingBlocks/Healthcare.Common/Healthcare.Common.csproj", "BuildingBlocks/Healthcare.Common/"]
COPY ["IdentityService/IdentityService.API/IdentityService.API.csproj", "IdentityService/IdentityService.API/"]
COPY ["IdentityService/IdentityService.Application/IdentityService.Application.csproj", "IdentityService/IdentityService.Application/"]
COPY ["IdentityService/IdentityService.Domain/IdentityService.Domain.csproj", "IdentityService/IdentityService.Domain/"]
COPY ["IdentityService/IdentityService.Infrastructure/IdentityService.Infrastructure.csproj", "IdentityService/IdentityService.Infrastructure/"]
COPY ["IdentityService/IdentityService.Contracts/IdentityService.Contracts.csproj", "IdentityService/IdentityService.Contracts/"]

RUN dotnet restore "IdentityService/IdentityService.API/IdentityService.API.csproj"

COPY BuildingBlocks/ BuildingBlocks/
COPY IdentityService/ IdentityService/

WORKDIR "/src/IdentityService/IdentityService.API"
RUN dotnet publish "IdentityService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "IdentityService.API.dll"]
