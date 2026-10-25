# Documentación de la API

La API del sistema está construida sobre ASP.NET Core utilizando arquitectura REST. Los endpoints están organizados bajo prefijos de ruta de versión (`/api/v1`) y de administración (`/api/admin`).

Todos los controladores (excepto los de catálogo público) requieren el paso del identificador del tenant para resolver el contexto de la tienda (mediante cookies o las cabeceras `X-Tenant-ID` o `X-Tenant-Slug`).

---

## 1. Inventario de Rutas por Módulo

Las rutas siguientes reflejan los prefijos y atributos HTTP declarados actualmente en los controladores. Se muestran rutas versionadas (`/api/v1`) y rutas legacy (`/api`) porque ambas están presentes en el backend. El acceso y los contratos específicos se detallan en las secciones posteriores.

### Autenticación (`AuthController`)
Prefijo: `/api/v1/auth`

- `POST /register` - Registrar usuario.
- `POST /login` - Iniciar sesión.
- `POST /logout` - Cerrar sesión.
- `POST /forgot-password` - Solicitar recuperación de contraseña.
- `POST /recover-with-code` - Recuperar acceso mediante código.

### Administración de usuarios (`AdminUsuariosController`)
Prefijo: `/api/admin/usuarios`

- `GET /` - Listar usuarios.
- `POST /` - Crear usuario.
- `PUT /{id}/rol` - Cambiar el rol de un usuario.
- `PUT /{id}/estado` - Cambiar el estado de un usuario.
- `POST /{id}/reset-password` - Generar códigos de recuperación.

### Usuarios de plataforma (`UsuariosV1Controller`)
Prefijo: `/api/v1/usuarios`

- `GET /` - Listar usuarios.
- `GET /{id}` - Obtener un usuario.
- `POST /invitar` - Invitar usuario.
- `PUT /{id}/rol` - Cambiar rol.
- `PUT /{id}/estado` - Cambiar estado.
- `DELETE /{id}` - Eliminar usuario.

### Carrito (`CarritoV1Controller`)
Prefijo: `/api/v1/carrito`

- `GET /` - Obtener el carrito.
- `POST /articulos` - Agregar un artículo.
- `PUT /articulos/{id}` - Actualizar un artículo.
- `DELETE /articulos/{id}` - Eliminar un artículo.

### Catálogo (`CatalogController`)
Prefijo: `/api`

- `GET /marcas` - Listar marcas.
- `GET /categorias` - Listar categorías.
- `GET /productos` - Listar productos.
- `GET /productos/buscar` - Buscar productos.
- `GET /productos/{id}` - Obtener un producto.
- `GET /seed/productos` - Ejecutar la ruta de inicialización de productos.
- `POST /productos` - Crear producto.
- `POST /productos/bulk` - Crear productos en lote.

### Inventarios (`InventariosController`)
Prefijo: `/api/v1/inventarios`

- `GET /sucursal/{sucursalId}` - Obtener inventario de una sucursal.
- `PUT /ajuste` - Ajustar inventario.

### Archivos y media (`MediaController`)
Prefijo: `/api/v1/media`

- `GET /cloudinary-signature` - Obtener firma para Cloudinary.
- `DELETE /` - Eliminar un recurso multimedia.

### Métodos de pago (`MetodoPagoController`)
Prefijo: `/api/metodoPago`

- `GET /config-stripe` - Obtener configuración pública de Stripe.
- `POST /contra-entrega` - Asegurar el método de pago contra entrega.
- `GET /` - Listar métodos de pago.
- `POST /` - Guardar método de pago.
- `DELETE /{id}` - Eliminar método de pago.

### Pagos (`PagosController`)
Prefijo: `/api/pagos`

- `POST /webhook` - Recibir eventos de Stripe.
- `POST /create-intent` - Crear un intento de pago.
- `GET /{paymentIntentId}/status` - Consultar el estado de un intento de pago.

### Productos (`ProductosV1Controller`)
Prefijo: `/api/v1/productos`

- `GET /` - Listar productos.
- `GET /{id}` - Obtener un producto.
- `POST /` - Crear producto.
- `POST /bulk` - Crear productos en lote.
- `PUT /{id}` - Actualizar producto.
- `DELETE /{id}` - Eliminar producto.

### Productos legacy (`ProductosController`)
Prefijo: `/api/productos`

- `PUT /{id}` - Actualizar producto.
- `DELETE /{id}` - Eliminar producto.

### Reportes administrativos (`ReportesController`)
Prefijo: `/api/admin/reportes`

- `GET /productos` - Consultar reporte de productos.
- `GET /empleados` - Consultar reporte de empleados.
- `GET /stock-critico` - Consultar reporte de stock crítico.
- `GET /demanda` - Consultar reporte de demanda.
- `GET /metodos-pago` - Consultar reporte de métodos de pago.

### Reportes de plataforma (`ReportesV1Controller`)
Prefijo: `/api/v1/reportes`

- `POST /ejecutar-raw` - Ejecutar una consulta de reporte.
- `POST /guardar` - Guardar un reporte.
- `GET /` - Listar reportes.
- `GET /{id}/correr` - Ejecutar un reporte guardado.

### Reservaciones legacy (`ReservacionController`)
Prefijo: `/api/reservacion`

- `POST /` - Crear reservación.
- `GET /` - Listar reservaciones.
- `GET /{id}` - Obtener una reservación.

### Reservaciones de plataforma (`ReservacionesV1Controller`)
Prefijo: `/api/v1/reservaciones`

- `POST /` - Crear reservación.
- `GET /mis-compras` - Consultar compras del usuario.
- `GET /control-staff` - Consultar reservaciones para staff.
- `PATCH /{id}/estado` - Cambiar el estado de una reservación.

### Sucursales (`SucursalesV1Controller`)
Prefijo: `/api/v1/sucursales`

- `GET /` - Listar sucursales.
- `GET /{id}` - Obtener una sucursal.
- `POST /` - Crear sucursal.
- `PUT /{id}` - Actualizar sucursal.
- `DELETE /{id}` - Eliminar sucursal.

### Tiendas y personalización (`TiendasController`)
Prefijo: `/api/v1/tiendas`

- `GET /` - Listar tiendas.
- `POST /` - Crear tienda.
- `GET /valida-slug/{slug}` - Comprobar disponibilidad del slug.
- `GET /{idOrSlug}` - Obtener tienda por identificador o slug.
- `PUT /actualizar` - Actualizar información de la tienda.
- `PUT /configuracion-visual` - Guardar configuración visual del constructor.
- `POST /integraciones` - Guardar integraciones de la tienda.
- `GET /integraciones` - Obtener integraciones de la tienda.

---

## 2. Detalle de Endpoints y Operaciones

### Auth (Sesiones y Registro) - `AuthController`
Prefijo: `/api/v1/auth`

* **`POST /login`** (Público)
  * **Payload**: `{ "correo": "string", "contrasena": "string" }`
  * **Respuesta (200 OK)**:
    ```json
    {
      "usuarioId": "string",
      "correo": "string",
      "nombre": "string",
      "tipoUsuario": "cliente|staff",
      "rol": "cajero|administrador|superadmin|null",
      "token": "jwt_token_string",
      "expiraEn": "ISO-8601-DateTime"
    }
    ```
* **`POST /register`** (Público / Admin para Staff)
  * **Payload**: `{ "correo": "string", "nombre": "string", "contrasena": "string", "tipoUsuario": "cliente|administrador", "rol": "cajero|administrador|null", "tipoCliente": "particular|mayorista|minorista" }`
  * **Respuesta (200 OK)**: Mismo formato de `/login`.
* **`POST /logout`** (Autenticado)
  * **Headers**: `Authorization: Bearer <token>`
  * **Respuesta (200 OK)**: `{ "mensaje": "Sesión cerrada correctamente" }`
* **`POST /forgot-password`** (Público)
  * **Payload**: `{ "correo": "string" }`
  * **Respuesta (200 OK)**: Envia el código OTP SMTP configurado de forma autónoma.
* **`POST /change-password`** (Autenticado)
  * **Payload**: `{ "contrasenaActual": "string", "nuevaContrasena": "string" }`
  * **Respuesta (200 OK)**: `{ "mensaje": "Contraseña cambiada con éxito" }`

---

### Administración de Usuarios - `AdminUsuariosController`
Prefijo: `/api/admin/usuarios` (Solo accesible por Staff con rol `administrador` o `superadmin`)

* **`GET /`**
  * **Respuesta (200 OK)**: Retorna lista de usuarios pertenecientes al tenant.
* **`POST /`**
  * **Payload**: `{ "nombre": "string", "email": "string", "tipoUsuario": "staff|cliente", "rolStaff": "cajero|admin", "sucursalId": "string|null" }`
  * **Respuesta (201 Created)**: Retorna el objeto del usuario creado.
* **`PUT /{id}`**
  * **Payload**: `{ "nombre": "string", "rolStaff": "string", "sucursalId": "string|null", "estado": boolean }`
  * **Respuesta (200 OK)**: Usuario actualizado.
* **`POST /{id}/reset-password`**
  * **Respuesta (200 OK)**: Genera 8 códigos de recuperación alternativos para restablecer la contraseña offline.
    ```json
    {
      "usuarioId": "string",
      "correo": "string",
      "nombre": "string",
      "codigos": ["code1", "code2", "code3", "..."]
    }
    ```

---

### Productos y Categorías - `ProductosV1Controller`
Prefijo: `/api/v1/productos` (Operaciones de escritura reservadas para Staff)

* **`GET /`** (Público)
  * **Parámetros**: `page` (int), `limit` (int), `categoriaId` (string), `buscar` (string)
  * **Respuesta (200 OK)**: Lista paginada de productos filtrados por el tenant actual.
* **`POST /`** (Admin)
  * **Payload**: `{ "nombre": "string", "descripcion": "string", "sku": "string", "precioMayoreo": decimal, "precioDetalle": decimal, "imagenUrl": "string", "categoriaId": "string" }`
  * **Respuesta (201 Created)**: Producto creado.
* **`PUT /{id}`** (Admin)
  * **Payload**: Estructura de actualización del producto.
  * **Respuesta (200 OK)**: Producto modificado.
* **`DELETE /{id}`** (Admin)
  * **Respuesta (200 OK)**: Soft-delete (marca columna `eliminado = true`).

---

### Reservaciones y Compras - `ReservacionesV1Controller`
Prefijo: `/api/v1/reservaciones`

* **`GET /`** (Staff)
  * **Respuesta (200 OK)**: Lista completa de reservaciones de la tienda.
* **`GET /mis-reservaciones`** (Cliente)
  * **Respuesta (200 OK)**: Lista de reservaciones asociadas al cliente logueado.
* **`POST /`** (Cliente)
  * **Payload**:
    ```json
    {
      "sucursalId": "string",
      "detalles": [
        { "productoId": "string", "cantidad": 5 }
      ]
    }
    ```
  * **Respuesta (201 Created)**: Reservación creada con estado de pago `"pendiente"`.
* **`PUT /{id}/despacho`** (Staff)
  * **Payload**: `{ "estadoDespacho": "procesando|completado|cancelado" }`
  * **Respuesta (200 OK)**: Estado actualizado.

---

### Inventarios - `InventariosController`
Prefijo: `/api/v1/inventarios` (Solo accesible por Staff)

* **`GET /sucursal/{sucursalId}`**
  * **Respuesta (200 OK)**: Existencias de todos los productos en la sucursal indicada.
* **`PUT /`**
  * **Payload**: `{ "sucursalId": "string", "productoId": "string", "stock": 50 }`
  * **Respuesta (200 OK)**: Existencia de stock actualizada en el inventario físico de la sucursal.

---

### Dashboard y Reportes - `ReportesV1Controller`
Prefijo: `/api/v1/reportes` (Solo accesible por Staff con rol `administrador` o superior)

* **`GET /dashboard`**
  * **Respuesta (200 OK)**: Métricas consolidadas (Ventas totales, reservaciones pendientes, stock bajo por producto, facturación del mes).
* **`POST /personalizados`**
  * **Payload**: `{ "nombre": "string", "querySql": "SELECT ... FROM ... WHERE tienda_id = @TiendaId" }`
  * **Respuesta (201 Created)**: Guarda una plantilla de consulta SQL personalizada para el tenant.
* **`GET /personalizados/{id}/ejecutar`**
  * **Respuesta (200 OK)**: Ejecuta de manera segura la consulta SQL personalizada del tenant y retorna la rejilla de datos en formato JSON.
* **`GET /ventas/exportar`**
  * **Respuesta (200 OK)**: Genera y descarga un archivo `.xlsx` (Excel) con el desglose de ventas del mes en curso utilizando `ClosedXML` u otra librería de hojas de cálculo.

---

### Pasarela de Pagos (Stripe) - `PagosController`
Prefijo: `/api/pagos`

* **`POST /crear-intento`** (Cliente Autenticado)
  * **Payload**: `{ "reservacionId": "string" }`
  * **Respuesta (200 OK)**: Retorna el `clientSecret` e `intentId` de Stripe para inicializar el SDK del lado del cliente.
* **`POST /webhook`** (Público)
  * **Payload**: Eventos asíncronos provenientes de los servidores de Stripe.
  * **Acción**: Intercepta eventos de tipo `payment_intent.succeeded`. Busca la reservación asociada mediante `stripe_intent_id`, cambia su estado a `"pagado"` en la base de datos, lo que dispara automáticamente la deducción de inventario (`SaveChangesAsync`) en la sucursal.

---

### Personalización de tienda e integraciones - `TiendasController`
Prefijo: `/api/v1/tiendas`

El controlador obtiene el tenant desde `X-Tenant-ID`, el tenant resuelto por middleware o el claim `tienda_id`. Las acciones de actualización e integraciones devuelven `400 Bad Request` con `{ "error": "Se requiere el header X-Tenant-ID." }` cuando no se puede resolver un tenant.

#### Obtener la configuración visual

No existe una ruta `GET /configuracion-visual` en el backend actual. Para obtener la tienda y su configuración se usa `GET /{idOrSlug}`. Si no existe la tienda, responde `404 Not Found` con `{ "error": "Tienda no encontrada." }`.

La respuesta es un `TiendaDto`; `configuracionVisual` se devuelve como una cadena que contiene el JSON almacenado:

```json
{
  "id": "tienda-id",
  "nombre": "Mi tienda",
  "slug": "mi-tienda",
  "estado": "activa",
  "configuracionVisual": "{\"theme\":{},\"pages\":[]}",
  "fechaCreacion": "2026-01-01T00:00:00Z",
  "credencialesIntegracion": {
    "tiendaId": "tienda-id",
    "stripePublicKey": null,
    "cloudinaryCloudName": null,
    "cloudinaryApiKey": null
  }
}
```

#### Guardar la configuración visual

* **`PUT /configuracion-visual`**
  * **Payload**: El objeto de configuración se envía dentro de la propiedad `configuracionVisual`. El backend almacena el JSON recibido.
    ```json
    {
      "configuracionVisual": {
        "theme": {},
        "pages": []
      }
    }
    ```
  * **Respuesta (200 OK)**: `TiendaDto` con la nueva configuración; `configuracionVisual` es una cadena con el JSON serializado.
  * **Error (400 Bad Request)**: No se pudo resolver el tenant.

#### Consultar integraciones

* **`GET /integraciones`**
  * **Respuesta (200 OK)**: `CredencialesIntegracionDtoFull`, con `tiendaId`, `stripeSecretKey`, `stripePublicKey`, `cloudinaryCloudName`, `cloudinaryApiKey`, `cloudinaryApiSecret`, `smtpEmail` y `smtpPassword`. Si todavía no hay credenciales guardadas, los campos de credenciales se devuelven como `null`.
  * **Error (400 Bad Request)**: No se pudo resolver el tenant.

#### Guardar integraciones

* **`POST /integraciones`**
  * **Payload**:
    ```json
    {
      "stripeSecretKey": null,
      "stripePublicKey": null,
      "cloudinaryCloudName": null,
      "cloudinaryApiKey": null,
      "cloudinaryApiSecret": null,
      "smtpEmail": null,
      "smtpPassword": null
    }
    ```
  * **Respuesta (200 OK)**: `TiendaDto`. Su propiedad `credencialesIntegracion` solo incluye `tiendaId`, `stripePublicKey`, `cloudinaryCloudName` y `cloudinaryApiKey`; no incluye los secretos.
  * **Error (400 Bad Request)**: No se pudo resolver el tenant.

**Nota de seguridad**: Aunque `GET /integraciones` devuelve secretos de Stripe, Cloudinary y SMTP, las acciones de este controlador solo verifican que exista contexto de tenant; no declaran una política o atributo explícito de autorización por rol. La definición Bearer de Swagger no aplica por sí sola autorización a una acción. Esta observación describe el código actual y requiere una revisión de seguridad independiente.

#### Agente IA del constructor

El backend actual no declara endpoints para un agente de IA dentro de la personalización de tienda. No se encontró un controlador, contrato de solicitud/respuesta ni servicio backend para generar o modificar la configuración visual con IA. Por tanto, no se documentan rutas hipotéticas; esta sección debe actualizarse cuando el endpoint exista y su contrato esté definido.

**Swagger**: La interfaz está disponible en `/swagger` únicamente cuando la API se ejecuta en el entorno `Development`.