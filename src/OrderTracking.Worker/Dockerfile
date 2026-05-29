# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar arquivos de projeto
COPY ["src/OrderTracking.Worker/OrderTracking.Worker.csproj", "OrderTracking.Worker/"]
COPY ["src/OrderTracking.Application/OrderTracking.Application.csproj", "OrderTracking.Application/"]
COPY ["src/OrderTracking.Domain/OrderTracking.Domain.csproj", "OrderTracking.Domain/"]
COPY ["src/OrderTracking.Infrastructure/OrderTracking.Infrastructure.csproj", "OrderTracking.Infrastructure/"]

# Restaurar dependências
RUN dotnet restore "OrderTracking.Worker/OrderTracking.Worker.csproj"

# Copiar todo o código fonte
COPY src/ .

# Build da aplicação
WORKDIR "/src/OrderTracking.Worker"
RUN dotnet build "OrderTracking.Worker.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "OrderTracking.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderTracking.Worker.dll"]
