# 1. ETAPA DE BUILD (Compilación)
# Utilizamos el SDK de .NET 9.0 para compilar el proyecto net9.0
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build 
WORKDIR /src

# Copia solo el archivo de proyecto para restaurar dependencias
# (Ajusta el nombre si tu archivo no es Ep_Linares.csproj)
COPY ["Ep_Linares.csproj", ""] 
RUN dotnet restore "Ep_Linares.csproj"

# Copia el resto del código fuente
COPY . .

# Publica la aplicación para producción
RUN dotnet publish "Ep_Linares.csproj" -c Release -o /app/publish

# 2. ETAPA FINAL (Ejecución)
# Usamos el Runtime de ASP.NET 9.0 (más ligero) para ejecutar la app
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Configuración del puerto para Render
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080 

# Copia los archivos publicados de la etapa de build
COPY --from=build /app/publish .

# Define el punto de entrada (Start Command)
# Aplica las migraciones de EF Core (para SQLite) y luego inicia la aplicación.
ENTRYPOINT ["/bin/bash", "-c", "dotnet ef database update && dotnet Ep_Linares.dll"]