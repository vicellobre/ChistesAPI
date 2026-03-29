# Enjoy — API REST (.NET)

Documentación del proyecto **Enjoy**: arquitectura limpia, configuración en `**appsettings.json`**, `**appsettings.Development.json`** y `**appsettings.Docker.json**`, y **uso con Docker Compose** (énfasis operativo). La API expone autenticación JWT, integración OAuth (GitHub/Google), chistes (APIs externas + Gemini), temas, matemáticas de demostración y notificaciones.

---

## Informe de cobertura de código

| | |
| :-- | :-- |
| **Informe en línea** | [**Abrir en GitHub Pages →**](https://vicellobre.github.io/ChistesAPI/) |
| **Qué muestra** | Cobertura de **pruebas unitarias** de la capa **Dominio** (`Enjoy.Domain`), generada con **Coverlet** y **ReportGenerator** (HTML interactivo). |
| **En el repo** | Tras regenerar: `CoverageResults/index.html` o `CoverageResults/Report/index.html`. |

---

## Tabla de contenidos

1. [Informe de cobertura de código](#informe-de-cobertura-de-código)
2. [Visión general](#1-visión-general)
3. [Arquitectura y proyectos](#2-arquitectura-y-proyectos)
4. [Requisitos previos](#3-requisitos-previos)
5. [Configuración en profundidad (`appsettings`)](#4-configuración-en-profundidad-appsettings) — [claves Gemini y GitHub](#413-obtener-claves-propias-gemini-y-github-oauth)
6. [Docker Compose — guía principal](#5-docker-compose--guía-principal)
7. [Desarrollo local sin Docker](#6-desarrollo-local-sin-docker)
8. [Casos de uso](#7-casos-de-uso)
9. [Rutas HTTP y ejemplos](#8-rutas-http-y-ejemplos)
10. [Swagger y OpenAPI](#9-swagger-y-openapi)
11. [Solución de problemas](#10-solución-de-problemas)
12. [Seguridad y secretos](#11-seguridad-y-secretos)
13. [Alcance, limitaciones y decisiones](#12-alcance-limitaciones-y-decisiones)

---

## 1. Visión general

**Enjoy** es una API ASP.NET Core que actúa como *composition root* (`Enjoy.API`) y delega reglas de negocio y acceso a datos en capas separadas. Incluye:

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
| `Enjoy.Infrastructure` | Servicios externos (Gemini, HTTP clients), autenticación JWT, etc.       |
| `Enjoy.Persistence`    | EF Core, repositorios, migraciones, *seeding* en desarrollo/Docker.      |
| `Enjoy.Domain`         | Entidades, value objects, errores de dominio, eventos.                   |


**Flujo típico:** `Controller` → `ISender` (MediatR) → *Handler* → repositorios / servicios → `Result` / errores tipados → respuesta HTTP o Problem Details.

---

## 3. Requisitos previos

- **.NET SDK 10** (el proyecto apunta a `net10.0`).
- **Docker Desktop** (o motor compatible) con **Docker Compose v2**.
- Opcional en local: **PostgreSQL** accesible (o solo usar el contenedor `enjoy-db`).

Todos los comandos de ejemplo asumen que el directorio de trabajo es la carpeta `**Enjoy`** (donde están `docker-compose.yml` y el código bajo `src/`).

---

## 4. Configuración en profundidad (`appsettings`)

En este proyecto la configuración se gestiona con `**appsettings.json`** y `**appsettings.{Environment}.json`** (p. ej. `**Development`** en local y `Docker` con Compose):

1. `appsettings.json`
2. `appsettings.{Environment}.json`

**Importante:** el JSON específico de entorno **sobrescribe** claves del base. Para secretos locales, usa `**appsettings.Development.json`** (o un archivo `*.local.json` ignorado por git si el repo es público; ver `.gitignore`); recuerda que docker-compose no lee User Secrets

**Patrón Options (`IOptions<T>`, `IOptionsSnapshot<T>`, etc.):** lee la configuración **ya fusionada**. Options es la forma tipada de consumir `IConfiguration` después de aplicar la precedencia anterior.

### 4.1. Entornos relevantes


| Entorno       | Cuándo                                                                                    | Archivo extra típico           |
| ------------- | ----------------------------------------------------------------------------------------- | ------------------------------ |
| `Development` | `dotnet run`, Visual Studio con perfil local                                              | `appsettings.Development.json` |
| `Docker`      | Contenedor definido en Compose (entorno `Docker` fijado en `docker-compose.override.yml`) | `appsettings.Docker.json`      |


En `Program.cs`, las **migraciones** y el **seed** de base de datos se ejecutan si el entorno es `**Development`** o `**Docker`** (no en `Production` por defecto).

### 4.2. Cadena de conexión (`ConnectionStrings:ConnectionEnjoy`)

- **Nombre** esperado por el código: `ConnectionEnjoy`.
- **Desarrollo (host):** suele apuntar a `localhost` y al puerto publicado de PostgreSQL (en este repo, **5433** en el host para no chocar con un Postgres local en 5432).
- **Docker (API en red con `enjoy-db`):** host `enjoy-db`, puerto **5432** (puerto interno del contenedor), no el mapeado al host.

Ejemplo conceptual **dentro de Docker**:

```text
Host=enjoy-db;Port=5432;Database=enjoydb;Username=postgres;Password=***
```

Esa cadena debe estar en `**appsettings.Docker.json**` (sección `ConnectionStrings:ConnectionEnjoy`). El `docker-compose.override.yml` del repositorio ya alinea la API con el host `enjoy-db` en la red de Compose.

### 4.3. JWT (`Jwt`)


| Clave                        | Descripción                                                                                                                                   |
| ---------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------- |
| `Issuer` / `Audience`        | Emisor y audiencia del token.                                                                                                                 |
| `SecretKey`                  | Clave simétrica HS256; debe tener **longitud suficiente** (p. ej. ≥ 32 caracteres). **No commitear** valores reales en repositorios públicos. |
| `TokenExpirationInMinutes`   | Vida del access token.                                                                                                                        |
| `RefreshTokenExpirationDays` | Vida del refresh token.                                                                                                                       |


Define `Jwt:SecretKey` en `**appsettings.Development.json`** (o en el `appsettings` del entorno que uses; ver §6).

### 4.4. Serilog

- **Consola:** siempre útil en desarrollo y contenedores.
- **Seq:** URL del servidor de ingestión. Debe coincidir con **dónde** corre Seq:
  - En **Compose**, el host DNS es el nombre del servicio (`enjoy-seq`) y el puerto **interno** de ingestión (p. ej. `5341`).
  - En **máquina local** con Seq mapeado al host, suele usarse `localhost` y el puerto publicado (p. ej. **5342** si el mapeo es `5342:5341`).

Si la URL es incorrecta, la API puede arrancar pero los logs no llegarán a Seq.

### 4.5. Clientes HTTP (`IntegrationHttpClients`)

- `**ChuckNorris`** / `**DadJoke`**: URLs públicas y timeouts acotados.
- `**ExternalAuth`**: cabeceras y timeouts para el intercambio con proveedores OAuth (token/userinfo según implementación).

### 4.6. OAuth (`ExternalAuth`)

Por proveedor (p. ej. **GitHub**, **Google**):

- `ClientId`, `ClientSecret`
- `RedirectUri` — debe coincidir **exactamente** con lo registrado en el proveedor y con la URL pública desde la que el usuario vuelve (p. ej. `http://localhost:8080/api/auth/external/callback?provider=GitHub` cuando la API está en el puerto 8080).

Si cambias el puerto o el dominio, actualiza **GitHub/Google Console** y la configuración.

### 4.7. Gemini (`Gemini`)

- `ApiKey`, `BaseUrl`, timeouts.
- `ModelId`: identificador del modelo en la **API de Gemini** (Google AI): el mismo `model` que documenta Google (modelos **Gemini** o **Gemma** expuestos por esa API, p. ej. `gemini-2.0-flash` o `gemma-3-12b-it`). La aplicación lo pasa tal cual al cliente; no es un alias interno. Si no configuras `ModelId`, el código usa un valor por defecto (p. ej. `gemma-3-12b-it`; revisa `GeminiJokeFusionService` si cambia).
- La clave es sensible; en repos públicos no subas valores reales: usa placeholders en lo versionado y rellena `Gemini:ApiKey` en `**appsettings.Development.json`** u otro archivo local no versionado.

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

Rutas típicas (notación con `:` como en documentación y variables de entorno con `__`):


| Ruta en configuración                                           | Uso                                                                                   |
| --------------------------------------------------------------- | ------------------------------------------------------------------------------------- |
| `ConnectionStrings:ConnectionEnjoy`                             | PostgreSQL                                                                            |
| `Jwt:SecretKey`                                                 | Firma JWT                                                                             |
| `Gemini:ApiKey`                                                 | Clave de la API Gemini (Google AI)                                                    |
| `Gemini:ModelId`                                                | Id de modelo en la API Gemini (Google): **Gemini** o **Gemma**, según la doc. oficial |
| `ExternalAuth:GitHub:ClientId` / `ClientSecret` / `RedirectUri` | OAuth GitHub                                                                          |
| `Serilog` → Seq `serverUrl`                                     | Destino de logs                                                                       |


### 4.13. Obtener claves propias (Gemini y GitHub OAuth)

Las credenciales **no se obtienen automáticamente**: debes crearlas en las consolas de Google y GitHub y copiarlas a `**appsettings.Development.json`** o `**appsettings.Docker.json`** según cómo ejecutes la API (plantilla opcional `secrets.example.json` si la hubiera en el repo).

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
  Pégala en `Gemini:ApiKey` en `**appsettings.Development.json`** o `**appsettings.Docker.json`**, según el entorno.
7. **Buenas prácticas**
  No subas la clave a repositorios públicos. En producción, restringe la clave (si la consola de Google lo permite: por HTTP referrer, IP, etc.) y rota la clave si se filtra.
8. **Modelo (`Gemini:ModelId`)**
  Debe ser un **id de modelo válido de la API Gemini** (Google AI), ya sea familia **Gemini** o **Gemma** listada para esa API: copia el identificador exacto de la [documentación de modelos](https://ai.google.dev/gemini-api/docs/models/gemini) (p. ej. `gemini-2.0-flash` o `gemma-3-12b-it`). Enjoy no traduce ni mapea ese valor: es el string que la API espera en las peticiones.

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
  En `**appsettings.Development.json`** o `**appsettings.Docker.json`**:
6. **Errores habituales**
  - **Redirect URI mismatch:** la URL en GitHub no es idéntica a la que usa el navegador al volver del login (incluye `http` vs `https`, puerto, barra final y query `?provider=GitHub`).  
  - **OAuth en organización:** si la app debe ser de una org, créala desde los ajustes de la organización o revisa restricciones de acceso.

---

## 5. Docker Compose — guía principal

El archivo `docker-compose.yml` define tres servicios coordinados. El archivo `docker-compose.override.yml` (cargado automáticamente por Compose) ajusta la API para el entorno **Docker** (entorno de hosting, URL de escucha y cadena de conexión acorde a `appsettings.Docker.json`).

### 5.1. Servicios


| Servicio    | Imagen / build                         | Función                                                                                  |
| ----------- | -------------------------------------- | ---------------------------------------------------------------------------------------- |
| `enjoy.api` | Build desde `src/Enjoy.API/Dockerfile` | API ASP.NET Core en el puerto **8080** del contenedor (mapeado al host).                 |
| `enjoy-db`  | `postgres:18`                          | Base de datos `enjoydb`, usuario/contraseña definidos en el propio `docker-compose.yml`. |
| `enjoy-seq` | `datalust/seq:latest`                  | Logs y trazas; UI y ingestión OTLP.                                                      |


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
- Alinea la cadena de conexión con el host `**enjoy-db`** (red interna de Compose), coherente con `ConnectionStrings:ConnectionEnjoy` en `appsettings.Docker.json`.

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

Los perfiles están en `Properties/launchSettings.json` (p. ej. `http` en **5046**, `https` en **7269** / **5046**). Ajusta en `**appsettings.Development.json`** la cadena `ConnectionStrings:ConnectionEnjoy` (p. ej. `Host=localhost;Port=5433;...` si usas Postgres del Compose), `Jwt:SecretKey` y el resto de claves que necesites (Gemini, OAuth, etc.) para alinear el mismo criterio que en Docker: todo en JSON por entorno.

---

## 7. Casos de uso


| Caso                          | Descripción                                                                                                              |
| ----------------------------- | ------------------------------------------------------------------------------------------------------------------------ |
| **Onboarding de equipo**      | `docker compose up -d --build`, abrir Swagger, probar login con usuario sembrado (`Database:DevelopmentUsers`).          |
| **Depuración de integración** | API en Docker + herramientas en host contra `localhost:8080` y DB en `localhost:5433`.                                   |
| **Observabilidad**            | Revisar logs en Seq; correlacionar con trazas OTLP si están configuradas.                                                |
| **OAuth GitHub**              | Registrar app en GitHub, alinear `RedirectUri` con `ExternalAuth:GitHub` y probar flujo `external/...-login` → callback. |
| **Chistes con IA**            | Endpoints que usan Gemini requieren `Gemini:ApiKey` y `Gemini:ModelId` acordes a la API Gemini, y cuotas de API.         |


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


| Síntoma                              | Posible causa                                                      | Qué revisar                                                                                                            |
| ------------------------------------ | ------------------------------------------------------------------ | ---------------------------------------------------------------------------------------------------------------------- |
| Error de puerto en uso al hacer `up` | Otro proceso ya escucha en **8080**, **5433**, **8081** o **5342** | Liberar el puerto o cambiar el mapeo en `docker-compose.yml` y alinear cadena de conexión / OAuth / Seq según §4 y §5. |
| API no arranca tras `up`             | Postgres no healthy                                                | `docker compose ps`, logs de `enjoy-db`                                                                                |
| 401 en rutas protegidas              | Token ausente o expirado; redirect HTTPS pierde header             | Renovar token; en Docker suele usarse solo HTTP en 8080                                                                |
| No hay logs en Seq                   | URL incorrecta en Serilog                                          | `enjoy-seq` vs `localhost` y puertos 5341/5342                                                                         |
| OAuth falla                          | Redirect URI distinta                                              | Consola del proveedor y `ExternalAuth`                                                                                 |
| Migraciones fallan                   | Cadena incorrecta                                                  | `ConnectionStrings:ConnectionEnjoy` en `appsettings` o `appsettings.Docker.json`                                       |


---

## 11. Seguridad y secretos

- **No** subas a repositorios públicos: claves JWT, `Gemini:ApiKey`, `ClientSecret` de OAuth, contraseñas de base de datos.
- En **desarrollo**, coloca valores locales en `**appsettings.Development.json`** o en archivos `*.local.json` ignorados por git. En **producción**, usa un almacén de secretos o variables de entorno del orquestador, no credenciales en JSON versionado.
- Cambia la contraseña de administrador de Seq y las credenciales de Postgres definidas en el `docker-compose` para despliegues reales.
- Revisa periódicamente **GitHub/Google** para rotar client secrets y restricciones de redirect URI.

---

## 12. Alcance, limitaciones y decisiones

Estado honesto del repositorio frente a tiempo disponible y prioridades. Sirve para alinear expectativas (revisiones, entrevistas, despliegues).

### Autenticación y usuarios

- **Registro estándar:** sí hay endpoint para registrar usuarios con **correo y contraseña** (flujo habitual).
- **Administradores:** no existe endpoint para **crear** administradores; solo el registro de usuarios en el sentido anterior. Los usuarios con rol **Admin** se crean mediante **seed** / valores fijos en configuración (`appsettings`, p. ej. `Database:DevelopmentUsers`), no vía API pública dedicada.
- **Google:** el **login con Google** **no está implementado** (aunque la configuración o el README mencionen OAuth de forma genérica, el flujo Google no está desarrollado).

### Pruebas

- **Unitarias:** no se alcanzó una cobertura global cercana al **90 %** por falta de tiempo; se cubrió en buena medida la capa **Dominio**, no el resto de capas por igual. El informe HTML publicado está enlazado arriba, en [**Informe de cobertura de código**](#informe-de-cobertura-de-código).
- **Integración:** no se desarrollaron **pruebas de integración** en este repositorio.
- **Email y SMS:** no hay pruebas automatizadas de los servicios de **correo** ni de **SMS**.
- **Conocimiento (frente al alcance anterior):** no obstante, **sí se maneja** el planteamiento de **pruebas unitarias** para **servicios convencionales** (dependencias sustituidas, SUT acotado) y de **pruebas de integración** centradas en **clientes HTTP** (`HttpClient`, handlers de prueba / *test doubles* para el pipeline de mensajes). Lo que faltó aquí fue **aplicar ese nivel de cobertura a todo el proyecto por tiempo**, no el criterio técnico.

### Errores y respuestas HTTP

- Por tiempo **no se uniformaron todos los errores posibles** con criterios de seguridad estrictos.  
**Ejemplo:** en el registro, si el **correo ya existe**, la API puede devolver un mensaje que permite **enumerar cuentas**; en producción convendría una respuesta **genérica** (indistinguible de otros casos) para no filtrar si el email está registrado.

### Secretos y `appsettings`

- El ecosistema .NET ofrece **User Secrets** para desarrollo local; aquí se priorizó `**appsettings.json`** y variantes por entorno por **velocidad de desarrollo** y porque **Docker Compose** no consume los user secrets del SDK `dotnet` de forma estándar en el contenedor. Para **producción** sigue siendo recomendable un almacén de secretos o variables inyectadas por el orquestador (véase §11).

---

*README orientado a operación y desarrollo del repositorio Enjoy. Ajusta nombres de servicios y puertos si personalizas el Compose.*
