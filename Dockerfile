# 1. ETAPA DE BUILD (Compilación)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build 
WORKDIR /src

COPY ["Ep_Linares.csproj", ""] 
RUN dotnet restore "Ep_Linares.csproj"

COPY . .

# Publica la aplicación
RUN dotnet publish "Ep_Linares.csproj" -c Release -o /app/publish

# 2. ETAPA FINAL (Ejecución)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# === CORRECCIÓN CLAVE: INSTALAR DOTNET EF EN LA IMAGEN FINAL ===
# La herramienta EF Core se necesita para el comando 'database update'
RUN dotnet tool install --global dotnet-ef --version 9.0.0-* # Usamos la versión 9.0
ENV PATH="${PATH}:/root/.dotnet/tools"
# ===============================================================

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080 

COPY --from=build /app/publish .

# El comando ahora encuentra 'dotnet ef' gracias a la instalación y la actualización del PATH
ENTRYPOINT ["/bin/bash", "-c", "dotnet ef database update && dotnet Ep_Linares.dll"]