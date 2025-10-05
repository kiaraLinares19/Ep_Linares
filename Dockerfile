# 1. ETAPA DE BUILD (Compilación)
# Utilizamos el SDK de .NET 9.0 para COMPILAR
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build 
WORKDIR /src

# Copia el archivo de proyecto y restaura dependencias
COPY ["Ep_Linares.csproj", ""] 
RUN dotnet restore "Ep_Linares.csproj"

# Copia el resto de los archivos fuente
COPY . .

# Publica la aplicación (compila todo y prepara para ejecución)
RUN dotnet publish "Ep_Linares.csproj" -c Release -o /app/publish

# 2. ETAPA FINAL (Ejecución)
# Usamos el Runtime de ASP.NET 9.0 (más ligero) para EJECUTAR
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Usamos Development temporalmente para ver el error real si no es de DB
ENV ASPNETCORE_ENVIRONMENT=Development 

# Configuración de puerto
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080 

# Copia los archivos publicados desde el stage 'build'
COPY --from=build /app/publish .

# La aplicación C# creará la DB y las tablas al iniciar.

# Inicia la aplicación
ENTRYPOINT ["dotnet", "Ep_Linares.dll"]