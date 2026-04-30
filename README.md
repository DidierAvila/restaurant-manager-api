# 🍽️ Restaurant Manager API

Sistema de gestión para restaurantes construido con **.NET 10** siguiendo **Clean Architecture** y patrones **CQRS**.

## 📋 Tabla de Contenidos

- [Características](#-características)
- [Arquitectura](#-arquitectura)
- [Tecnologías](#-tecnologías)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Módulos Implementados](#-módulos-implementados)
- [Configuración](#-configuración)
- [Instalación](#-instalación)
- [Migraciones de Base de Datos](#-migraciones-de-base-de-datos)
- [Uso](#-uso)
- [Endpoints API](#-endpoints-api)
- [Roadmap](#-roadmap)

---

## ✨ Características

- ✅ **Clean Architecture** (Onion Architecture) con 4 capas bien definidas
- ✅ **CQRS** - Separación de Commands y Queries con MediatR
- ✅ **Repository Pattern** - Abstracción del acceso a datos
- ✅ **JWT Authentication** - Autenticación basada en tokens
- ✅ **RBAC** - Control de acceso basado en roles y permisos
- ✅ **AutoMapper** - Mapeo automático entre entidades y DTOs
- ✅ **Entity Framework Core** - ORM con PostgreSQL
- ✅ **Swagger/OpenAPI** - Documentación automática de la API

---

## 🏗️ Arquitectura

El proyecto sigue **Clean Architecture** con las siguientes capas:

```
┌─────────────────────────────────────────────────────────────┐
│                    RestaurantManager.Api                     │
│                  (Presentation Layer)                        │
│              Controllers, Middleware, Program.cs             │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│                RestaurantManager.Application                 │
│                   (Application Layer)                        │
│    Features (Commands, Queries, Handlers), DTOs, Mappings   │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│                  RestaurantManager.Domain                    │
│                     (Domain Layer)                           │
│              Entities, Enums, Repository Interfaces          │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│              RestaurantManager.Infrastructure                │
│                 (Infrastructure Layer)                       │
│       DbContext, Repository Implementations, Migrations      │
└─────────────────────────────────────────────────────────────┘
```

---

## 🛠️ Tecnologías

| Categoría | Tecnología | Versión |
|-----------|-----------|---------|
| **Framework** | .NET | 10.0 |
| **Base de Datos** | PostgreSQL | - |
| **ORM** | Entity Framework Core | 10.0.7 |
| **Provider** | Npgsql.EntityFrameworkCore.PostgreSQL | 10.0.1 |
| **Mediator** | MediatR | 14.1.0 |
| **Mapping** | AutoMapper | 12.0.1 |
| **Autenticación** | JWT (System.IdentityModel.Tokens.Jwt) | 8.17.0 |
| **Hashing** | BCrypt.Net-Next | 4.1.0 |
| **Documentación** | Swashbuckle.AspNetCore (Swagger) | 10.1.7 |

---

## 📁 Estructura del Proyecto

```
restaurant-manager-api/
├── RestaurantManager.Api/
│   ├── Controllers/
│   │   ├── AccessControl/      # Roles, Permisos, Usuarios, Auth
│   │   ├── DishesController.cs # Gestión de platos
│   │   ├── OrdersController.cs # Gestión de pedidos
│   │   └── ReportsController.cs # Reportes de ventas
│   ├── Extensions/
│   │   └── ExtencionServices.cs # Registro de servicios DI
│   └── Program.cs
│
├── RestaurantManager.Application/
│   ├── Features/
│   │   ├── AccessControl/      # CQRS para roles y permisos
│   │   ├── Dishes/             # CQRS para platos
│   │   │   ├── Commands/
│   │   │   ├── Queries/
│   │   │   └── Handlers/
│   │   ├── Orders/             # CQRS para pedidos
│   │   │   ├── Commands/
│   │   │   ├── Queries/
│   │   │   └── Handlers/
│   │   └── Reports/            # Queries para reportes
│   ├── DTOs/
│   │   ├── AccessControl/
│   │   ├── Dishes/
│   │   ├── Orders/
│   │   └── Reports/
│   └── Mappings/
│       ├── AccessControl/
│       └── Restaurant/         # Profiles de AutoMapper
│
├── RestaurantManager.Domain/
│   ├── Entities/
│   │   ├── AccessControl/      # User, Role, Permission, etc.
│   │   ├── Dish.cs
│   │   ├── DishCategory.cs     # Enum
│   │   ├── Order.cs
│   │   ├── OrderItem.cs
│   │   └── OrderStatus.cs      # Enum
│   └── Repositories/
│       └── IRepositoryBase.cs
│
└── RestaurantManager.Infrastructure/
    ├── DbContexts/
    │   ├── RestaurantManagerDbContext.cs
    │   └── DesignTimeDbContextFactory.cs
    ├── Repositories/
    │   └── RepositoryBase.cs
    └── Migrations/
```

---

## 🎯 Módulos Implementados

### 1. Control de Acceso (Access Control) ✅

Sistema completo de autenticación y autorización:

- **Usuarios** - CRUD completo de usuarios con tipos
- **Roles** - Gestión de roles con permisos asignables
- **Permisos** - Sistema granular de permisos
- **Autenticación JWT** - Login con tokens Bearer
- **Sesiones** - Gestión de sesiones activas
- **Menús** - Menús de navegación dinámicos

### 2. Gestión de Platos (Dishes) ✅

Módulo completo del menú del restaurante:

**Entidades:**
- `Dish` - Platos con nombre, descripción, precio, categoría, disponibilidad
- `DishCategory` - Enum: Entradas, PlatosFuertes, Sopas, Bebidas, Postres

**Funcionalidades:**
- ✅ CRUD completo de platos
- ✅ Validación de nombre único
- ✅ Toggle de disponibilidad
- ✅ Filtrado por categoría
- ✅ Eliminación protegida (no permite si tiene pedidos)

### 3. Gestión de Pedidos (Orders) ✅

Sistema de pedidos con máquina de estados:

**Entidades:**
- `Order` - Pedido con mesa (1-50), mesero, fecha, estado
- `OrderItem` - Detalle del pedido (plato, cantidad, precio, notas)
- `OrderStatus` - Enum con flujo: Abierto → En Preparación → Listo → Entregado → Cerrado

**Funcionalidades:**
- ✅ Crear pedidos asociados a mesas
- ✅ Agregar/quitar platos (cantidad 1-20)
- ✅ Validación: no pedidos duplicados en la misma mesa
- ✅ Avanzar estado del pedido
- ✅ Cálculo automático de totales
- ✅ Consultar pedidos activos

### 4. Reportes de Ventas (Reports) ✅

Sistema de reportes y analíticas:

**Funcionalidades:**
- ✅ Reporte de ventas por rango de fechas
- ✅ Total de ventas y pedidos
- ✅ Ticket promedio
- ✅ Plato estrella (más vendido)
- ✅ Ventas por categoría con porcentajes
- ✅ Detalle de ventas por plato
- ✅ Solo cuenta pedidos "Entregado" o "Cerrado"

---

## ⚙️ Configuración

### Requisitos Previos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [PostgreSQL](https://www.postgresql.org/download/) (o Docker)
- [Entity Framework Core Tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet)

### Configuración de Base de Datos

Actualiza `appsettings.json` con tu cadena de conexión:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost; Port=5432; Database=restaurant_manager; Username=postgres; Password=admin"
  },
  "JwtSettings": {
    "key": "tu-clave-secreta-super-segura-aqui"
  }
}
```

**⚠️ Importante:** En producción, mueve los secretos a **User Secrets** o **Azure Key Vault**.

---

## 📦 Instalación

### 1. Clonar el repositorio

```bash
git clone <url-del-repositorio>
cd restaurant-manager-api
```

### 2. Restaurar paquetes

```bash
dotnet restore
```

### 3. Compilar el proyecto

```bash
dotnet build
```

---

## 🧑‍💻 Desarrollo local (paso a paso)

### 1) Base de datos

Elige una opción:

**Opción A: PostgreSQL local (puerto 5432)**

- Asegura que Postgres esté corriendo en `localhost:5432`.
- Crea la base de datos `restaurant_manager` (o ajusta la cadena de conexión).
- Configura la cadena en `RestaurantManager.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost; Port=5432; Database=restaurant_manager; Username=postgres; Password=admin"
  }
}
```

**Opción B: PostgreSQL en Docker (puerto 5433)**

```bash
docker compose up -d db
```

### 2) Ejecutar migraciones

**Local (5432):**

```bash
dotnet ef database update --project RestaurantManager.Infrastructure --startup-project RestaurantManager.Api --context RestaurantManagerDbContext
```

**Docker (5433, PowerShell):**

```powershell
$env:ConnectionStrings__DefaultConnection='Host=localhost; Port=5433; Database=restaurant_manager; Username=postgres; Password=admin'
dotnet ef database drop --project RestaurantManager.Infrastructure --startup-project RestaurantManager.Api --context RestaurantManagerDbContext --force
dotnet ef database update --project RestaurantManager.Infrastructure --startup-project RestaurantManager.Api --context RestaurantManagerDbContext
```

### 3) Ejecutar la API

**Local:**

```bash
cd RestaurantManager.Api
dotnet run
```

**Docker (con perfil de lanzamiento):**

```bash
cd RestaurantManager.Api
dotnet run --launch-profile docker
```

### 4) Validar que quedó OK

- Swagger: `https://localhost:7044/swagger`
- Tablas seed esperadas: `menu_items` (25), `orders` (3), `order_details` (4)

---

## 🗄️ Migraciones de Base de Datos

### Crear migración (si modificas el modelo)

```bash
dotnet ef migrations add NombreDeLaMigracion --project RestaurantManager.Infrastructure --startup-project RestaurantManager.Api --context RestaurantManagerDbContext
```

### Aplicar migraciones (Postgres local en 5432)

```bash
dotnet ef database update --project RestaurantManager.Infrastructure --startup-project RestaurantManager.Api --context RestaurantManagerDbContext
```

### Aplicar migraciones (Postgres en Docker en 5433)

Levantar la DB:

```bash
docker compose up -d db
```

Ejecutar migraciones apuntando al puerto 5433 (PowerShell):

```powershell
$env:ConnectionStrings__DefaultConnection='Host=localhost; Port=5433; Database=restaurant_manager; Username=postgres; Password=admin'
dotnet ef database drop --project RestaurantManager.Infrastructure --startup-project RestaurantManager.Api --context RestaurantManagerDbContext --force
dotnet ef database update --project RestaurantManager.Infrastructure --startup-project RestaurantManager.Api --context RestaurantManagerDbContext
```

Alternativa: correr la API usando el perfil `docker` (ya configura `ConnectionStrings__DefaultConnection`):

```bash
cd RestaurantManager.Api
dotnet run --launch-profile docker
```

### Ver historial de migraciones (DB configurada por ConnectionStrings__DefaultConnection)

```bash
dotnet ef migrations list --project RestaurantManager.Infrastructure --startup-project RestaurantManager.Api --context RestaurantManagerDbContext
```

---

## 🚀 Uso

### Ejecutar la API

```bash
cd RestaurantManager.Api
dotnet run
```

La API estará disponible en:
- **HTTP:** `http://localhost:5018`
- **HTTPS:** `https://localhost:7044`
- **Swagger UI:** `https://localhost:7044/swagger`

### CORS Configurado

La API permite solicitudes desde:
- `http://localhost:3001`
- `https://localhost:4200`

---

## 📡 Endpoints API

### Access Control

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/auth/login` | Autenticación con JWT |
| GET | `/api/roles` | Listar todos los roles |
| POST | `/api/roles` | Crear nuevo rol |
| PUT | `/api/roles/{id}` | Actualizar rol |
| DELETE | `/api/roles/{id}` | Eliminar rol |
| GET | `/api/permissions` | Listar permisos |
| POST | `/api/permissions` | Crear permiso |
| GET | `/api/users` | Listar usuarios |
| POST | `/api/users` | Crear usuario |

### Dishes (Platos)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/dishes` | Listar todos los platos (con filtros opcionales) |
| GET | `/api/dishes/{id}` | Obtener plato por ID |
| GET | `/api/dishes/available` | Listar platos disponibles |
| GET | `/api/dishes/category/{category}` | Filtrar por categoría |
| POST | `/api/dishes` | Crear nuevo plato |
| PUT | `/api/dishes/{id}` | Actualizar plato |
| DELETE | `/api/dishes/{id}` | Eliminar plato |
| PATCH | `/api/dishes/{id}/toggle-availability` | Cambiar disponibilidad |

### Orders (Pedidos)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/orders` | Listar todos los pedidos |
| GET | `/api/orders/{id}` | Obtener pedido por ID |
| GET | `/api/orders/active` | Listar pedidos activos |
| GET | `/api/orders/table/{tableNumber}` | Obtener pedido activo de una mesa |
| POST | `/api/orders` | Crear nuevo pedido |
| POST | `/api/orders/{orderId}/items` | Agregar plato al pedido |
| DELETE | `/api/orders/{orderId}/items/{itemId}` | Quitar plato del pedido |
| PATCH | `/api/orders/{id}/advance-status` | Avanzar estado del pedido |

### Reports (Reportes)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/reports/sales?fromDate={date}&toDate={date}` | Reporte de ventas por rango de fechas |

---

## 🎯 Roadmap

### Próximas Funcionalidades

- [ ] **Validación con FluentValidation**
- [ ] **Tests Unitarios** (xUnit + Moq)
- [ ] **Middleware de manejo global de errores**
- [ ] **Paginación en todos los endpoints**
- [ ] **Logging con Serilog**
- [ ] **Caching con Redis**
- [ ] **Rate Limiting**
- [ ] **Refresh Tokens**
- [ ] **Exportación de reportes (CSV, PDF)**
- [ ] **SignalR para notificaciones en tiempo real**
- [ ] **Gestión de mesas**
- [ ] **Inventario de ingredientes**

---

## 📝 Notas de Migración

Este proyecto es una **migración de una aplicación legacy** construida en ASP.NET Web Forms con SQL Server. La migración incluye:

✅ De SQL directo → Entity Framework Core con Repository Pattern
✅ De concatenación de strings → LINQ y expresiones lambda
✅ De Code-Behind → Clean Architecture con CQRS
✅ De SQL Server → PostgreSQL
✅ De validación básica → Validaciones robustas en handlers

---

## 📄 Licencia

Este proyecto está bajo licencia privada para uso interno.

---

## 👥 Contribución

Para contribuir al proyecto:

1. Crea una rama con tu feature: `git checkout -b feature/nueva-funcionalidad`
2. Commitea tus cambios: `git commit -m 'feat: agregar nueva funcionalidad'`
3. Push a la rama: `git push origin feature/nueva-funcionalidad`
4. Abre un Pull Request

---

## 📞 Contacto

Para preguntas o soporte, contacta al equipo de desarrollo.
