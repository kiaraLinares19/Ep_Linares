# 1. ETAPA DE BUILD (Compilación)
# Usa la imagen del SDK de .NET 8 (necesaria para compilar y publicar)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build 
WORKDIR /src

# Copia solo el archivo de proyecto para restaurar dependencias
# Esto optimiza el caché de Docker si solo cambian los archivos de código fuente
COPY ["Ep_Linares.csproj", ""] 
RUN dotnet restore "Ep_Linares.csproj"

# Copia el resto del código fuente
COPY . .

# Publica la aplicación para producción. Los archivos listos quedan en /app/publish
RUN dotnet publish "Ep_Linares.csproj" -c Release -o /app/publish

# 2. ETAPA FINAL (Ejecución)
# Usa la imagen ASP.NET Runtime (mucho más ligera) para correr la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Define que la aplicación Kestrel escuchará en el puerto 8080 (el puerto estándar de Render para Docker)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080 

# Copia los archivos publicados de la etapa de build
COPY --from=build /app/publish .

# Define el punto de entrada (Start Command)
# Ejecuta la migración de la base de datos (SQLite) y, si tiene éxito (&&), inicia la aplicación.
ENTRYPOINT ["/bin/bash", "-c", "dotnet ef database update && dotnet Ep_Linares.dll"]