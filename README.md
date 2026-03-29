# Enjoy — API REST (.NET)

Documentación del proyecto **Enjoy**: arquitectura limpia, configuración en **`appsettings`** y ***user secrets***, y **uso con Docker Compose** (énfasis operativo). La API expone autenticación JWT, integración OAuth (GitHub/Google), chistes (APIs externas + Gemini), temas, matemáticas de demostración y notificaciones.

---

## Tabla de contenidos

1. [Visión general](#1-visión-general)
2. [Arquitectura y proyectos](#2-arquitectura-y-proyectos)
3. [Requisitos previos](#3-requisitos-previos)
4. [Configuración en profundidad (`appsettings`)](#4-configuración-en-profundidad-appsettings) — [claves Gemini y GitHub](#413-obtener-claves-propias-gemini-y-github-oauth)
5. [Docker Compose — guía principal](#5-docker-compose--guía-principal)
6. [Desarrollo local sin Docker](#6-desarrollo-local-sin-docker)
7. [Casos de uso](#7-casos-de-uso)
8. [Rutas HTTP y ejemplos](#8-rutas-http-y-ejemplos)
9. [Swagger y OpenAPI](#9-swagger-y-openapi)
10. [Solución de problemas](#10-solución-de-problemas)
11. [Seguridad y secretos](#11-seguridad-y-secretos)

---

## 1. Visión general

**Enjoy** es una API ASP.NET Core que actúa como *composition root* (`Enjoy.API`) y delega reglas de negocio y acceso a datos en capas separadas (Application, Domain, Infrastructure, Persistence, Presentation). Incluye:

- **Identity + EF Core** con PostgreSQL.
- **JWT** (access + refresh) y **login externo** (OAuth).
- **Chistes**: Chuck Norris, Dad Jokes, combinación con **Gemini**, y CRUD sobre datos propios.
- **Serilog** con consola y **Seq** (agregación de logs).
- **OpenTelemetry** exportando OTLP (en Compose, hacia el contenedor Seq).
- **Rate limiting** diferenciando usuarios autenticados / anónimos.

---

## 2. Arquitectura y proyectos


| Proyecto               | Rol                                                                      |
| ---------------------- | ------------------------------------------------------------------------ |
| `Enjoy.API`            | Host: DI, pipeline HTTP, Serilog, OpenTelemetry, carga de configuración. |
| `Enjoy.Presentation`   | Controladores, modelos HTTP, middlewares compartidos.                    |
| `Enjoy.Application`    | Casos de uso (MediatR), validación, contratos de aplicación.             |
| `Enjoy.Domain`         | Entidades, value objects, errores de dominio, eventos.                   |
| `Enjoy.Infrastructure` | Servicios externos (Gemini, HTTP clients), autenticación JWT, etc.       |
| `Enjoy.Persistence`    | EF Core, repositorios, migraciones, *seeding* en desarrollo/Docker.      |


**Flujo típico:** `Controller` → `ISender` (MediatR) → *Handler* → repositorios / servicios → `Result` / errores tipados → respuesta HTTP o Problem Details.

---

## 3. Requisitos previos

- **.NET SDK 10** (el proyecto apunta a `net10.0`).
- **Docker Desktop** (o motor compatible) con **Docker Compose v2**.
- Opcional en local: **PostgreSQL** accesible (o solo usar el contenedor `enjoy-db`).

Todos los comandos de ejemplo asumen que el directorio de trabajo es la carpeta `**Enjoy`** (donde están `docker-compose.yml` y el código bajo `src/`).

---

## 4. Configuración en profundidad (`appsettings`)

En este proyecto la configuración se gestiona sobre todo con **`appsettings.json`**, archivos **`appsettings.{Environment}.json`** y, en desarrollo local, ***user secrets***. ASP.NET Core fusiona esas fuentes en este orden (los últimos valores **ganan**):

1. `appsettings.json`
2. `appsettings.{Environment}.json` (p. ej. `Development`, `Docker`)
3. *User secrets* (solo en desarrollo, si están configurados)

**Importante:** primero se cargan los `appsettings` (base y por entorno); después los *user secrets* **sobrescriben** claves coincidentes. No es al revés: los *user secrets* no van “antes” que los JSON.

**Patrón Options (`IOptions<T>`, `IOptionsSnapshot<T>`, etc.):** lee la configuración **ya fusionada**. Options es la forma tipada de consumir `IConfiguration` después de aplicar la precedencia anterior.

### 4.1. Entornos relevantes


| Entorno       | Cuándo                                                           | Archivo extra típico           |
| ------------- | ---------------------------------------------------------------- | ------------------------------ |
| `Development` | `dotnet run`, Visual Studio con perfil local                     | `appsettings.Development.json` |
| `Docker`      | Contenedor definido en Compose (entorno `Docker` fijado en `docker-compose.override.yml`) | `appsettings.Docker.json`      |


En `Program.cs`, las **migraciones** y el **seed** de base de datos se ejecutan si el entorno es `**Development`** o `**Docker**` (no en `Production` por defecto).

### 4.2. Cadena de conexión (`ConnectionStrings:ConnectionEnjoy`)

- **Nombre** esperado por el código: `ConnectionEnjoy`.
- **Desarrollo (host):** suele apuntar a `localhost` y al puerto publicado de PostgreSQL (en este repo, **5433** en el host para no chocar con un Postgres local en 5432).
- **Docker (API en red con `enjoy-db`):** host `enjoy-db`, puerto **5432** (puerto interno del contenedor), no el mapeado al host.

Ejemplo conceptual **dentro de Docker**:

```text
Host=enjoy-db;Port=5432;Database=enjoydb;Username=postgres;Password=***
```

Esa cadena debe estar en **`appsettings.Docker.json`** (sección `ConnectionStrings:ConnectionEnjoy`). El `docker-compose.override.yml` del repositorio ya alinea la API con el host `enjoy-db` en la red de Compose.

### 4.3. JWT (`Jwt`)


| Clave                        | Descripción                                                                                                                                   |
| ---------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------- |
| `Issuer` / `Audience`        | Emisor y audiencia del token.                                                                                                                 |
| `SecretKey`                  | Clave simétrica HS256; debe tener **longitud suficiente** (p. ej. ≥ 32 caracteres). **No commitear** valores reales en repositorios públicos. |
| `TokenExpirationInMinutes`   | Vida del access token.                                                                                                                        |
| `RefreshTokenExpirationDays` | Vida del refresh token.                                                                                                                       |


Para no versionar la clave en disco, usa *user secrets* con la clave `Jwt:SecretKey` (ver §6).

### 4.4. Serilog

- **Consola:** siempre útil en desarrollo y contenedores.
- **Seq:** URL del servidor de ingestión. Debe coincidir con **dónde** corre Seq:
  - En **Compose**, el host DNS es el nombre del servicio (`enjoy-seq`) y el puerto **interno** de ingestión (p. ej. `5341`).
  - En **máquina local** con Seq mapeado al host, suele usarse `localhost` y el puerto publicado (p. ej. **5342** si el mapeo es `5342:5341`).

Si la URL es incorrecta, la API puede arrancar pero los logs no llegarán a Seq.

### 4.5. Clientes HTTP (`IntegrationHttpClients`)

- `**ChuckNorris`** / `**DadJoke**`: URLs públicas y timeouts acotados.
- `**ExternalAuth**`: cabeceras y timeouts para el intercambio con proveedores OAuth (token/userinfo según implementación).

### 4.6. OAuth (`ExternalAuth`)

Por proveedor (p. ej. **GitHub**, **Google**):

- `ClientId`, `ClientSecret`
- `RedirectUri` — debe coincidir **exactamente** con lo registrado en el proveedor y con la URL pública desde la que el usuario vuelve (p. ej. `http://localhost:8080/api/auth/external/callback?provider=GitHub` cuando la API está en el puerto 8080).

Si cambias el puerto o el dominio, actualiza **GitHub/Google Console** y la configuración.

### 4.7. Gemini (`Gemini`)

- `ApiKey`, `ModelId`, `BaseUrl`, timeouts.
- La clave es sensible; en repos públicos conviene *user secrets* u omitir el valor en el JSON versionado.

### 4.8. CORS (`Cors`)

- `AllowedOrigins`: orígenes del front (p. ej. Vite en `http://localhost:5173`).
- `AllowAnyOriginWhenEmpty`: en `Development` puede relajar CORS cuando la lista está vacía (revisar `appsettings.Development.json`).

### 4.9. Rate limiting (`RateLimiting`)

Límites separados para usuarios **autenticados** y **anónimos** (ventanas, colas, códigos 429). Ajustar según pruebas de carga y UX.

### 4.10. Base de datos al arranque (`Database`)


| Opción                     | Efecto                                                                                |
| -------------------------- | ------------------------------------------------------------------------------------- |
| `ApplyMigrationsOnStartup` | Aplica migraciones EF al iniciar (solo `Development` / `Docker` en el código actual). |
| `SeedIdentityRoles`        | Crea roles de Identity (p. ej. User, Admin).                                          |
| `SeedDevelopmentUsers`     | Crea usuarios desde `DevelopmentUsers` si no existen.                                 |
| `SeedDemoData`             | Datos demo (p. ej. topics/jokes de prueba) si aplica.                                 |
| `DevelopmentUsers`         | Lista de `{ Name, Email, Password, Role }` para seed.                                 |


### 4.11. OpenTelemetry / OTLP

En **Docker Compose**, el servicio `enjoy.api` en `docker-compose.yml` ya envía trazas al Seq de la misma red (`enjoy-seq`). Si cambias nombres de servicio o puertos internos, revisa ese archivo y la URL de Seq.

### 4.12. Resumen: claves habituales en JSON

Rutas típicas (notación con `:` como en *user secrets* / documentación):


| Ruta en configuración | Uso |
| --------------------- | --- |
| `ConnectionStrings:ConnectionEnjoy` | PostgreSQL |
| `Jwt:SecretKey` | Firma JWT |
| `Gemini:ApiKey` | API Gemini (también en `appsettings` si aplica) |
| `ExternalAuth:GitHub:ClientId` / `ClientSecret` / `RedirectUri` | OAuth GitHub |
| `Serilog` → Seq `serverUrl` | Destino de logs |


### 4.13. Obtener claves propias (Gemini y GitHub OAuth)

Las credenciales **no se obtienen automáticamente**: debes crearlas en las consolas de Google y GitHub y copiarlas a `appsettings` o a *user secrets* (plantilla opcional `secrets.example.json`).

#### Gemini — API key (Google AI)

La API de modelos generativos de Google se gestiona desde **Google AI Studio** (clave pensada para desarrollo y pruebas con la API Gemini).

1. **Cuenta Google**  
   Inicia sesión con una cuenta de Google en el navegador.

2. **Abrir Google AI Studio**  
   Entra en [Google AI Studio](https://aistudio.google.com/) (`https://aistudio.google.com`).

3. **Crear o ver API keys**  
   En el menú lateral o en la sección de API, usa **Get API key** / **Create API key** (el texto exacto puede variar según la versión de la interfaz).

4. **Proyecto en Google Cloud**  
   La primera vez, el asistente puede pedirte **crear o seleccionar un proyecto** en Google Cloud y habilitar la facturación solo si el modelo o el uso lo exigen (muchas cuentas tienen capa gratuita con límites; revisa la documentación actual de Google).

5. **Generar la clave**  
   Crea una clave nueva, asígnale un nombre reconocible (p. ej. `enjoy-local`) y **cópiala** en un lugar seguro; Google puede mostrarla una sola vez o permitir verla de nuevo según el flujo.

6. **Enlazar con Enjoy**  
   Pégala en `Gemini:ApiKey` en `appsettings` o en *user secrets*.

7. **Buenas prácticas**  
   No subas la clave a repositorios públicos. En producción, restringe la clave (si la consola de Google lo permite: por HTTP referrer, IP, etc.) y rota la clave si se filtra.

8. **Modelo (`ModelId`)**  
   El `ModelId` en configuración debe ser un identificador **válido para la API que estés usando**. Si cambias de modelo, revisa la [documentación de modelos de Gemini](https://ai.google.dev/gemini-api/docs/models/gemini).

#### GitHub — OAuth App (`ClientId` y `ClientSecret`)

Sirven para que los usuarios inicien sesión con GitHub. Debes registrar una **OAuth App** y copiar el par **Client ID** + **Client secret**.

1. **Entrar en la configuración de desarrolladores**  
   Con tu cuenta de GitHub, ve a **Settings** → **Developer settings** → **OAuth Apps**, o abre directamente:  
   [https://github.com/settings/developers](https://github.com/settings/developers).

2. **Registrar una nueva aplicación**  
   Pulsa **New OAuth App** y completa al menos:
   - **Application name:** nombre visible (p. ej. `Enjoy local`).
   - **Homepage URL:** URL base de tu app (p. ej. `http://localhost:8080` o la de tu front).
   - **Authorization callback URL:** debe coincidir **carácter a carácter** con `ExternalAuth:GitHub:RedirectUri` en la API. Ejemplo típico con la API en Docker en el puerto 8080:

     ```text
     http://localhost:8080/api/auth/external/callback?provider=GitHub
     ```

     Si usas otro puerto o HTTPS (p. ej. perfil `https` de Visual Studio), registra **esa** URL exacta en GitHub y la misma en `ExternalAuth:GitHub:RedirectUri`.

3. **Registrar**  
   Guarda la aplicación. Verás el **Client ID** (público).

4. **Generar Client secret**  
   En la página de la OAuth App, usa **Generate a new client secret**. **Cópialo en el momento**: GitHub puede mostrar el secreto completo solo al crearlo; guárdalo en un gestor de secretos.

5. **Enlazar con Enjoy**  
   En `appsettings` o *user secrets*:

   ```json
   "ExternalAuth": {
     "GitHub": {
       "ClientId": "TU_CLIENT_ID",
       "ClientSecret": "TU_CLIENT_SECRET",
       "RedirectUri": "http://localhost:8080/api/auth/external/callback?provider=GitHub"
     }
   }
   ```

6. **Errores habituales**  
   - **Redirect URI mismatch:** la URL en GitHub no es idéntica a la que usa el navegador al volver del login (incluye `http` vs `https`, puerto, barra final y query `?provider=GitHub`).  
   - **OAuth en organización:** si la app debe ser de una org, créala desde los ajustes de la organización o revisa restricciones de acceso.

---

## 5. Docker Compose — guía principal

El archivo `docker-compose.yml` define tres servicios coordinados. El archivo `docker-compose.override.yml` (cargado automáticamente por Compose) ajusta la API para el entorno **Docker** (entorno de hosting, URL de escucha y cadena de conexión acorde a `appsettings.Docker.json`).

### 5.1. Servicios


| Servicio    | Imagen / build                         | Función                                                                       |
| ----------- | -------------------------------------- | ----------------------------------------------------------------------------- |
| `enjoy.api` | Build desde `src/Enjoy.API/Dockerfile` | API ASP.NET Core en el puerto **8080** del contenedor (mapeado al host).      |
| `enjoy-db`  | `postgres:18`                          | Base de datos `enjoydb`, usuario/contraseña definidos en el propio `docker-compose.yml`. |
| `enjoy-seq` | `datalust/seq:latest`                  | Logs y trazas; UI y ingestión OTLP.                                           |


### 5.2. Puertos en el host (referencia)


| Host     | Destino         | Uso                                                                                                                        |
| -------- | --------------- | -------------------------------------------------------------------------------------------------------------------------- |
| **8080** | API `8080`      | HTTP de la API (`/swagger`, `/api/...`).                                                                                   |
| **5433** | Postgres `5432` | Conexión desde herramientas en el **host** (pgAdmin, `psql`, API corriendo fuera de Docker con cadena a `localhost:5433`). |
| **5342** | Seq OTLP `5341` | Ingestión OTLP desde el host (p. ej. perfil `http` en `launchSettings`).                                                   |
| **8081** | Seq web `80`    | Interfaz web de Seq (`http://localhost:8081`).                                                                             |


> **Nota:** Dentro de la red Docker, la API usa **enjoy-db:5432** y **enjoy-seq:5341**, no los puertos del host.

### 5.3. Volúmenes y datos persistentes

- `./.containers/pgdata` → datos de PostgreSQL (supervivencia entre `down` y nuevos `up`).
- `./.containers/seq_data` → datos de Seq.

Conviene **excluir** `.containers/` del control de versiones si no se desea versionar datos locales (añadir a `.gitignore` si aplica).

### 5.4. Dependencias y arranque

- `enjoy.api` **depende** de `enjoy-db` con condición `**service_healthy`** (Postgres listo antes de que la API aplique migraciones).
- `enjoy-seq` debe estar **iniciado** antes o junto con la API para OTLP/Serilog según configuración.

### 5.5. Qué aporta `docker-compose.override.yml`

- Fija el entorno de hosting **Docker** para que se cargue `appsettings.Docker.json`.
- Define la escucha HTTP en el puerto **8080** del contenedor.
- Alinea la cadena de conexión con el host **`enjoy-db`** (red interna de Compose), coherente con `ConnectionStrings:ConnectionEnjoy` en `appsettings.Docker.json`.

### 5.6. Comandos esenciales

Desde la carpeta `**Enjoy`**:

```bash
# Construir imágenes y levantar todo en segundo plano
docker compose up -d --build

# Ver logs de la API (seguimiento)
docker compose logs -f enjoy.api

# Detener y eliminar contenedores (los volúmenes nombrados en el archivo persisten en ./.containers)
docker compose down

# Reconstruir solo la API tras cambios de código
docker compose up -d --build enjoy.api
```

### 5.7. Comprobaciones tras `up`

1. **API:** abrir `http://localhost:8080/swagger` (en entornos no productivos Swagger está habilitado).
2. **Seq:** abrir `http://localhost:8081` e iniciar sesión con la contraseña de administrador inicial definida en el servicio Seq del `docker-compose.yml` (cambiarla en despliegues reales).
3. **Postgres:** desde el host, con cliente SQL, `localhost:5433` y las credenciales del servicio `enjoy-db`.

### 5.8. Caso: API en Docker y front en el host

- CORS: añadir el origen del front (p. ej. `http://localhost:5173`) en `Cors:AllowedOrigins` en el `appsettings` que corresponda.
- OAuth: las **redirect URIs** deben apuntar a la URL donde el navegador llama a la API (p. ej. `http://localhost:8080/...`).

### 5.9. Caso: ejecutar la API en el host y Postgres en Docker

- Levantar solo la base: `docker compose up -d enjoy-db` (y opcionalmente Seq).
- Cadena de conexión: `Host=localhost;Port=5433;...` (puerto publicado en el host).
- Ejecutar la API con entorno **Development** para cargar `appsettings.Development.json` y tener Serilog/Seq alineados con `localhost:5342` si usas OTLP hacia Seq en el host.

---

## 6. Desarrollo local sin Docker

```bash
cd src/Enjoy.API
dotnet restore
dotnet run --launch-profile https
```

Los perfiles están en `Properties/launchSettings.json` (p. ej. `http` en **5046**, `https` en **7269** / **5046**). Ajusta la cadena de conexión a tu instancia de PostgreSQL.

**User Secrets** (el proyecto define `UserSecretsId` en el `.csproj`):

```bash
dotnet user-secrets set "ConnectionStrings:ConnectionEnjoy" "Host=localhost;Port=5433;..." --project src/Enjoy.API/Enjoy.API.csproj
dotnet user-secrets set "Jwt:SecretKey" "TU_CLAVE_LARGA" --project src/Enjoy.API/Enjoy.API.csproj
```

---

## 7. Casos de uso


| Caso                          | Descripción                                                                                                              |
| ----------------------------- | ------------------------------------------------------------------------------------------------------------------------ |
| **Onboarding de equipo**      | `docker compose up -d --build`, abrir Swagger, probar login con usuario sembrado (`Database:DevelopmentUsers`).          |
| **Depuración de integración** | API en Docker + herramientas en host contra `localhost:8080` y DB en `localhost:5433`.                                   |
| **Observabilidad**            | Revisar logs en Seq; correlacionar con trazas OTLP si están configuradas.                                                |
| **OAuth GitHub**              | Registrar app en GitHub, alinear `RedirectUri` con `ExternalAuth:GitHub` y probar flujo `external/...-login` → callback. |
| **Chistes con IA**            | Endpoints que usan Gemini requieren `Gemini:ApiKey` válida y cuotas de API.                                              |


---

## 8. Rutas HTTP y ejemplos

Prefijos base (todos bajo el mismo host, p. ej. `http://localhost:8080`):


| Área            | Prefijo                      |
| --------------- | ---------------------------- |
| Autenticación   | `/api/auth`                  |
| Usuario / Admin | `/api/usuario`, `/api/admin` |
| Chistes         | `/api/chistes`               |
| Temas           | `/api/temas`                 |
| Matemáticas     | `/api/matematicas`           |
| Notificaciones  | `/api/notificaciones`        |


La mayoría de rutas de negocio requieren `**Authorization: Bearer <access_token>`** y roles `User` o `Admin` según el endpoint.

### 8.1. Login (obtener JWT)

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@localhost.dev",
  "password": "Admin123!"
}
```

*(La contraseña debe coincidir con la configurada en `Database:DevelopmentUsers` tras el seed; cámbiala en producción.)*

### 8.2. Llamada autenticada

```bash
curl -s -H "Authorization: Bearer TU_ACCESS_TOKEN" "http://localhost:8080/api/chistes/aleatorio"
```

### 8.3. OAuth (visión general)

1. `GET` al endpoint de inicio de login externo del proveedor expuesto por la API (ver Swagger).
2. El usuario autoriza en GitHub/Google.
3. Redirección a `/api/auth/external/callback` con `code` y `state`.
4. La API emite tokens JWT según la implementación actual.

---

## 9. Swagger y OpenAPI

Con entorno **no** `Production`, la app usa **Swagger UI** (ruta típica `/swagger`). Úsalo para:

- Descubrir rutas, esquemas y códigos de respuesta.
- Probar requests con el botón **Authorize** si configuras el esquema JWT en la UI.

---

## 10. Solución de problemas


| Síntoma                  | Posible causa                                          | Qué revisar                                             |
| ------------------------ | ------------------------------------------------------ | ------------------------------------------------------- |
| API no arranca tras `up` | Postgres no healthy                                    | `docker compose ps`, logs de `enjoy-db`                 |
| 401 en rutas protegidas  | Token ausente o expirado; redirect HTTPS pierde header | Renovar token; en Docker suele usarse solo HTTP en 8080 |
| No hay logs en Seq       | URL incorrecta en Serilog                              | `enjoy-seq` vs `localhost` y puertos 5341/5342          |
| OAuth falla              | Redirect URI distinta                                  | Consola del proveedor y `ExternalAuth`                  |
| Migraciones fallan       | Cadena incorrecta                                      | `ConnectionStrings:ConnectionEnjoy` en `appsettings` o `appsettings.Docker.json` |


---

## 11. Seguridad y secretos

- **No** subas a repositorios públicos: claves JWT, `Gemini:ApiKey`, `ClientSecret` de OAuth, contraseñas de base de datos.
- En **producción**, evita secretos en archivos versionados: usa *user secrets* en desarrollo y un almacén seguro acorde a tu plataforma en despliegue.
- Cambia la contraseña de administrador de Seq y las credenciales de Postgres definidas en el `docker-compose` para despliegues reales.
- Revisa periódicamente **GitHub/Google** para rotar client secrets y restricciones de redirect URI.

---

*README orientado a operación y desarrollo del repositorio Enjoy. Ajusta nombres de servicios y puertos si personalizas el Compose.*