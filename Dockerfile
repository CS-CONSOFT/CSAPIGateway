# Fase base para execução do serviço
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Fase para compilar o projeto
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CSGateway.csproj", "."]
RUN dotnet restore "./CSGateway.csproj"
COPY . . 
WORKDIR "/src/."
RUN dotnet build "./CSGateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Fase de publicação do projeto
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CSGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Fase final para a execução do serviço
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Definir argumentos que podem ser passados para o contêiner
ARG API_HOST_BS
ARG API_PORT_BS

ARG API_HOST_ST
ARG API_PORT_ST

# Configurar variáveis de ambiente dentro do contêiner
ENV API_HOST_BS=${API_HOST_BS} \
    API_PORT_BS=${API_PORT_BS} \
    API_HOST_ST=${API_HOST_ST} \
    API_PORT_ST=${API_PORT_ST}

# Configurar a entrada do serviço
ENTRYPOINT ["dotnet", "CSGateway.dll"]
