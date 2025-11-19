FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Microservices/UserManagement/UserManagement.API/UserManagement.API.csproj", "Microservices/UserManagement/UserManagement.API/"]
COPY ["Microservices/UserManagement/UserManagement.Application/UserManagement.Application.csproj", "Microservices/UserManagement/UserManagement.Application/"]
COPY ["Microservices/UserManagement/UserManagement.Domain/UserManagement.Domain.csproj", "Microservices/UserManagement/UserManagement.Domain/"]
COPY ["Microservices/UserManagement/UserManagement.Infrastructure/UserManagement.Infrastructure.csproj", "Microservices/UserManagement/UserManagement.Infrastructure/"]
RUN dotnet restore "Microservices/UserManagement/UserManagement.API/UserManagement.API.csproj"
COPY . .
WORKDIR "/src/Microservices/UserManagement/UserManagement.API"
RUN dotnet build "./UserManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UserManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserManagement.API.dll"]
