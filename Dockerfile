# 1. ETAPA DE BUILD (Compilación y Migración)
# Utilizamos el SDK de .NET 9.0
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build 
WORKDIR /src

COPY ["Ep_Linares.csproj", ""] 
RUN dotnet restore "Ep_Linares.csproj"

COPY . .

# Publica la aplicación
RUN dotnet publish "Ep_Linares.csproj" -c Release -o /app/publish

# --- PASO CLAVE: GENERAR EL ARCHIVO SQLite MIGRADO EN LA ETAPA DE BUILD ---
# Ejecutamos la migración. Render debe encontrar el EF tool aquí (en el SDK).
# Esto crea el archivo 'portalacademico.db' dentro de la carpeta /app/publish.
WORKDIR /app/publish
RUN dotnet Ep_Linares.dll database update
# -------------------------------------------------------------------------

# 2. ETAPA FINAL (Ejecución)
# Usamos el Runtime de ASP.NET 9.0 (no necesitamos el SDK aquí, solo la aplicación)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080 

# Copia los archivos publicados, incluyendo el archivo SQLite migrado.
COPY --from=build /app/publish .

# Inicia la aplicación (ya no necesita ejecutar la migración, solo iniciar la app)
ENTRYPOINT ["dotnet", "Ep_Linares.dll"]