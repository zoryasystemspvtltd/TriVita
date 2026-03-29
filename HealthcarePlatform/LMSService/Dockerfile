# docker build -f LMSService/Dockerfile .

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["BuildingBlocks/Healthcare.Common/Healthcare.Common.csproj", "BuildingBlocks/Healthcare.Common/"]
COPY ["LMSService/LMSService.API/LMSService.API.csproj", "LMSService/LMSService.API/"]
COPY ["LMSService/LMSService.Application/LMSService.Application.csproj", "LMSService/LMSService.Application/"]
COPY ["LMSService/LMSService.Domain/LMSService.Domain.csproj", "LMSService/LMSService.Domain/"]
COPY ["LMSService/LMSService.Infrastructure/LMSService.Infrastructure.csproj", "LMSService/LMSService.Infrastructure/"]
COPY ["LMSService/LMSService.Contracts/LMSService.Contracts.csproj", "LMSService/LMSService.Contracts/"]

RUN dotnet restore "LMSService/LMSService.API/LMSService.API.csproj"

COPY BuildingBlocks/ BuildingBlocks/
COPY LMSService/ LMSService/

WORKDIR "/src/LMSService/LMSService.API"
RUN dotnet publish "LMSService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LMSService.API.dll"]
