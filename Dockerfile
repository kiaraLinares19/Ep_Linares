# 1. ETAPA DE BUILD (Compilación y Migración)
# Utilizamos el SDK de .NET 9.0
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build 
WORKDIR /src

# --- PASO 1: INSTALAR DOTNET EF TOOL ---
# Instalamos la herramienta global 'dotnet-ef'
RUN dotnet tool install --global dotnet-ef --version 9.0.0-* ENV PATH="${PATH}:/root/.dotnet/tools"
# ----------------------------------------

COPY ["Ep_Linares.csproj", ""] 
RUN dotnet restore "Ep_Linares.csproj"

COPY . .

# --- PASO 2: EJECUTAR MIGRACIÓN DESDE EL DIRECTORIO RAIZ (/src) ---
# Ejecutamos la migración. El archivo SQLite (ej. app.db) se creará en /src.
RUN dotnet ef database update --project Ep_Linares.csproj --startup-project Ep_Linares.csproj
# -------------------------------------------------------------------

# Publica la aplicación
RUN dotnet publish "Ep_Linares.csproj" -c Release -o /app/publish

# 2. ETAPA FINAL (Ejecución)
# Usamos el Runtime de ASP.NET 9.0 (más ligero)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Configuración de entorno para ver errores detallados si es necesario
ENV ASPNETCORE_ENVIRONMENT=Development 

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080 

# Copia 1: Copia los archivos publicados desde /app/publish del stage 'build'
COPY --from=build /app/publish .

# --- COPIA 2 (CLAVE): Copiar el archivo de base de datos generado ---
# Copiamos cualquier archivo *.db que se haya generado en /src (por la migración) 
# al directorio de trabajo final (/app). Esto asegura que la DB esté presente.
# Si tu archivo DB tiene un nombre específico (ej. Ep_Linares.db), puedes usar ese nombre en lugar de *.db
RUN cp /src/*.db . || echo "Advertencia: No se encontró ningún archivo *.db para copiar."
# ---------------------------------------------------------------------

# Inicia la aplicación
ENTRYPOINT ["dotnet", "Ep_Linares.dll"]