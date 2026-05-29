# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar arquivos de projeto
COPY ["src/OrderTracking.Api/OrderTracking.Api.csproj", "OrderTracking.Api/"]
COPY ["src/OrderTracking.Application/OrderTracking.Application.csproj", "OrderTracking.Application/"]
COPY ["src/OrderTracking.Domain/OrderTracking.Domain.csproj", "OrderTracking.Domain/"]
COPY ["src/OrderTracking.Infrastructure/OrderTracking.Infrastructure.csproj", "OrderTracking.Infrastructure/"]

# Restaurar dependências
RUN dotnet restore "OrderTracking.Api/OrderTracking.Api.csproj"

# Copiar todo o código fonte
COPY src/ .

# Build da aplicação
WORKDIR "/src/OrderTracking.Api"
RUN dotnet build "OrderTracking.Api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "OrderTracking.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderTracking.Api.dll"]
