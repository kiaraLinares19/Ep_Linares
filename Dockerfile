
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build 
WORKDIR /src

# --- PASO CLAVE 1: INSTALAR DOTNET EF TOOL EN EL SDK ---
RUN dotnet tool install --global dotnet-ef --version 9.0.0-* ENV PATH="${PATH}:/root/.dotnet/tools"
# --------------------------------------------------------

COPY ["Ep_Linares.csproj", ""] 
RUN dotnet restore "Ep_Linares.csproj"

COPY . .

# Publica la aplicación
RUN dotnet publish "Ep_Linares.csproj" -c Release -o /app/publish

# --- PASO CLAVE 2: Ejecutamos la migración con la herramienta instalada ---
# Mantenemos la migración en el build para generar el archivo SQLite.
WORKDIR /app/publish
RUN dotnet ef database update --project /src/Ep_Linares.csproj --startup-project /src/Ep_Linares.csproj
# Nota: Apuntamos explícitamente a /src, aunque el .csproj debería ser visible.
# -------------------------------------------------------------------------

# 2. ETAPA FINAL (Ejecución)
# Usamos el Runtime de ASP.NET 9.0 (más ligero)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080 

# Copia los archivos publicados, incluyendo el archivo SQLite migrado.
COPY --from=build /app/publish .

# Inicia la aplicación. La migración ya está hecha.
ENTRYPOINT ["dotnet", "Ep_Linares.dll"]