# 1. ETAPA DE BUILD (Compilación y Migración)
# Utilizamos el SDK de .NET 9.0
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build 
WORKDIR /src

COPY ["Ep_Linares.csproj", ""] 
RUN dotnet restore "Ep_Linares.csproj"

COPY . .

# Publica la aplicación
RUN dotnet publish "Ep_Linares.csproj" -c Release -o /app/publish

# --- CORRECCIÓN CLAVE: Usamos 'dotnet ef' en la etapa de build para migrar ---
# La herramienta 'dotnet ef' migra y se detiene (no inicia el host)
WORKDIR /app/publish
# Usamos 'dotnet ef database update' que está disponible en el SDK
RUN dotnet ef database update --project Ep_Linares.csproj --startup-project Ep_Linares.csproj
# -------------------------------------------------------------------------

# 2. ETAPA FINAL (Ejecución)
# Usamos el Runtime de ASP.NET 9.0 (más ligero)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080 

# Copia los archivos publicados, incluyendo el archivo SQLite migrado.
COPY --from=build /app/publish .

# Inicia la aplicación. Ya no necesita correr la migración.
ENTRYPOINT ["dotnet", "Ep_Linares.dll"]