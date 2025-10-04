# Ep_Linares - Portal Académico

Este es un proyecto web desarrollado con ASP.NET Core 9.0 y SQLite, que implementa un portal académico con gestión de cursos y matrículas. Incluye un panel de coordinador con roles para CRUD de cursos y control de matrículas.

## Características

- Registro e inicio de sesión de usuarios usando ASP.NET Identity.
- Panel de Coordinador con autorización basada en roles.
- CRUD de cursos: crear, editar, desactivar.
- Gestión de matrículas por curso: confirmar o cancelar.
- Lista de cursos disponibles para los estudiantes.
- Sesión en Redis para almacenar datos temporales (último curso visitado, etc.).
- Despliegue preparado para Render como Web Service.

## Requisitos

- .NET 9.0 SDK
- SQLite
- Redis 
- Visual Studio / VS Code o similar

## Despliegue en Render

Variables mínimas de entorno:

- `ASPNETCORE_ENVIRONMENT=Production`
- `ASPNETCORE_URLS=http://0.0.0.0:${PORT}`
- `ConnectionStrings__DefaultConnection`
- `Redis__ConnectionString`

El proyecto está configurado para ser desplegado desde la rama `deploy/render`.

## Instalación local

1. Clonar el repositorio:  
   ```bash
   git clone <https://github.com/kiaraLinares19/Ep_Linares.git>
   cd Ep_Linares
Url de Render https://ep-linares.onrender.com
