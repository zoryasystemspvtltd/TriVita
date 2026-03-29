# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CommunicationService/CommunicationService.API/CommunicationService.API.csproj", "CommunicationService/CommunicationService.API/"]
COPY ["CommunicationService/CommunicationService.Application/CommunicationService.Application.csproj", "CommunicationService/CommunicationService.Application/"]
COPY ["CommunicationService/CommunicationService.Domain/CommunicationService.Domain.csproj", "CommunicationService/CommunicationService.Domain/"]
COPY ["CommunicationService/CommunicationService.Infrastructure/CommunicationService.Infrastructure.csproj", "CommunicationService/CommunicationService.Infrastructure/"]
COPY ["CommunicationService/CommunicationService.Contracts/CommunicationService.Contracts.csproj", "CommunicationService/CommunicationService.Contracts/"]
COPY ["BuildingBlocks/Healthcare.Common/Healthcare.Common.csproj", "BuildingBlocks/Healthcare.Common/"]
COPY ["BuildingBlocks/Healthcare.Swagger/Healthcare.Swagger.csproj", "BuildingBlocks/Healthcare.Swagger/"]
RUN dotnet restore "CommunicationService/CommunicationService.API/CommunicationService.API.csproj"
COPY . .
WORKDIR "/src/CommunicationService/CommunicationService.API"
RUN dotnet publish "CommunicationService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CommunicationService.API.dll"]
