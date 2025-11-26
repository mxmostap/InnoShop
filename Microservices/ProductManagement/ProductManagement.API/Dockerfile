FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Microservices/ProductManagement/ProductManagement.API/ProductManagement.API.csproj", "Microservices/ProductManagement/ProductManagement.API/"]
COPY ["Microservices/ProductManagement/ProductManagement.Application/ProductManagement.Application.csproj", "Microservices/ProductManagement/ProductManagement.Application/"]
COPY ["Microservices/ProductManagement/ProductManagement.Domain/ProductManagement.Domain.csproj", "Microservices/ProductManagement/ProductManagement.Domain/"]
COPY ["Microservices/ProductManagement/ProductManagement.Infrastructure/ProductManagement.Infrastructure.csproj", "Microservices/ProductManagement/ProductManagement.Infrastructure/"]
RUN dotnet restore "Microservices/ProductManagement/ProductManagement.API/ProductManagement.API.csproj"
COPY . .
WORKDIR "/src/Microservices/ProductManagement/ProductManagement.API"
RUN dotnet build "./ProductManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProductManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductManagement.API.dll"]
