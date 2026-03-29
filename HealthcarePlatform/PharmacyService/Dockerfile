# docker build -f PharmacyService/Dockerfile .

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["BuildingBlocks/Healthcare.Common/Healthcare.Common.csproj", "BuildingBlocks/Healthcare.Common/"]
COPY ["PharmacyService/PharmacyService.API/PharmacyService.API.csproj", "PharmacyService/PharmacyService.API/"]
COPY ["PharmacyService/PharmacyService.Application/PharmacyService.Application.csproj", "PharmacyService/PharmacyService.Application/"]
COPY ["PharmacyService/PharmacyService.Domain/PharmacyService.Domain.csproj", "PharmacyService/PharmacyService.Domain/"]
COPY ["PharmacyService/PharmacyService.Infrastructure/PharmacyService.Infrastructure.csproj", "PharmacyService/PharmacyService.Infrastructure/"]
COPY ["PharmacyService/PharmacyService.Contracts/PharmacyService.Contracts.csproj", "PharmacyService/PharmacyService.Contracts/"]

RUN dotnet restore "PharmacyService/PharmacyService.API/PharmacyService.API.csproj"

COPY BuildingBlocks/ BuildingBlocks/
COPY PharmacyService/ PharmacyService/

WORKDIR "/src/PharmacyService/PharmacyService.API"
RUN dotnet publish "PharmacyService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PharmacyService.API.dll"]
