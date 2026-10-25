# Documentación de la API

La API del sistema está construida sobre ASP.NET Core utilizando arquitectura REST. Los endpoints están organizados bajo prefijos de ruta de versión (`/api/v1`) y de administración (`/api/admin`).

El contexto multi-tenant se resuelve por solicitud, no de forma uniforme para todos los controladores. El middleware acepta `X-Tenant-ID`, `X-Tenant-Slug` o un subdominio de tienda; algunas acciones también obtienen el tenant desde el claim `tienda_id`. Cada sección indica los requisitos que aplica su controlador. No todas las rutas exigen tenant.

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
* **`POST /register`** (Público; el registro de administradores aplica controles adicionales)
  * **Payload**: `{ "correo": "string", "nombre": "string", "contrasena": "string", "tipoUsuario": "string", "apellido": "string|null", "telefono": "string|null", "tipoCliente": "string|null", "rol": "string|null", "direccion": "string|null" }`
  * **Respuesta (201 Created)**: `AuthResponseDto`, con identificador, correo, nombre, tipo de usuario, rol, tipo de cliente, token, expiración y `esSuperAdmin`.
* **`POST /logout`** (Autenticado)
  * **Headers**: `Authorization: Bearer <token>`
  * **Respuesta (204 No Content)**: Sesión cerrada.
* **`POST /forgot-password`** (Público)
  * **Headers**: `X-Tenant-ID` obligatorio para esta acción.
  * **Payload**: `{ "correo": "string" }`
  * **Respuesta (200 OK)**: `{ "mensaje": "Se ha enviado un código de verificación a tu correo electrónico." }`.
  * **Error (400 Bad Request)**: Falta `X-Tenant-ID`, no hay configuración SMTP, no se encuentra el cliente o no se pudo enviar el correo.
* **`POST /recover-with-code`** (Público)
  * **Payload**: `{ "correo": "string", "codigo": "string", "nuevaContrasena": "string" }`
  * **Respuesta (200 OK)**: `{ "mensaje": "Contraseña actualizada correctamente. Inicia sesión con tu nueva contraseña." }`.
  * **Error (400 Bad Request)**: Datos faltantes, contraseña con menos de ocho caracteres o código inválido, vencido o usado.

---

### Administración de Usuarios - `AdminUsuariosController`
Prefijo: `/api/admin/usuarios` (Solo accesible por Staff con rol `administrador` o `superadmin`)

* **`GET /`**
  * **Respuesta (200 OK)**: Retorna lista de usuarios pertenecientes al tenant.
* **`POST /`**
  * **Payload**: `{ "nombre": "string", "email": "string", "tipoUsuario": "staff|cliente", "rolStaff": "cajero|admin", "sucursalId": "string|null" }`
  * **Respuesta (201 Created)**: Retorna el objeto del usuario creado.
* **`PUT /{id}/rol`** (Superadmin)
  * **Payload**: `CambiarRolUsuarioRequestDto`.
  * **Respuesta (200 OK)**: Usuario actualizado; **403 Forbidden** si el solicitante no es superadmin.
* **`PUT /{id}/estado`** (Administrador)
  * **Payload**: `CambiarEstadoRequestDto`, con la propiedad `estado`.
  * **Respuesta (200 OK)**: Usuario actualizado; **404 Not Found** si no existe.
* **`POST /{id}/reset-password`**
  * **Respuesta (200 OK)**: Genera ocho códigos de recuperación de un solo uso, mostrados en texto plano únicamente en esta respuesta y almacenados hasheados.
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
Prefijo: `/api/v1/productos` (Las acciones verifican tenant; el controlador no valida un rol de Staff)

* **`GET /`** (Público)
  * **Requisito**: Tenant resoluble; si falta, responde **400 Bad Request**.
  * **Respuesta (200 OK)**: Lista de productos del tenant.
* **`GET /{id}`**
  * **Respuesta (200 OK)**: Producto; **404 Not Found** si no existe.
* **`POST /`** (Admin)
  * **Payload**: `{ "nombre": "string", "precioMayoreo": 0, "precioDetalle": 0, "categoriaId": "string|null", "sku": "string|null", "descripcion": "string|null", "imagenUrl": "string|null", "publicado": true, "stockMinimo": 0, "stockSucursales": [] }`.
  * **Respuesta (201 Created)**: Producto creado.
* **`POST /bulk`**
  * **Payload**: Arreglo de productos con nombre, precios, categoría, SKU, descripción, imagen, publicación, stock mínimo y stock actual.
  * **Respuesta (201 Created)**: Productos creados.
* **`PUT /{id}`** (Admin)
  * **Payload**: Mismos campos editables de `POST /`, incluyendo `stockSucursales`.
  * **Respuesta (200 OK)**: Producto actualizado; **404 Not Found** si no existe.
* **`DELETE /{id}`** (Admin)
  * **Respuesta (204 No Content)**: Producto eliminado; **404 Not Found** si no existe.

---

### Reservaciones y Compras - `ReservacionesV1Controller`
Prefijo: `/api/v1/reservaciones`

* **`POST /`** (Cliente autenticado y tenant resoluble)
  * **Payload**: `{ "sucursalId": "string", "stripeIntentId": "string|null" }`.
  * **Respuesta (201 Created)**: Reservación creada.
  * **Error (401 Unauthorized)**: Falta identidad de usuario o tenant.
* **`GET /mis-compras`** (Cliente autenticado y tenant resoluble)
  * **Respuesta (200 OK)**: Compras asociadas al usuario autenticado.
  * **Error (401 Unauthorized)**: Falta identidad de usuario o tenant.
* **`GET /control-staff`** (Staff con tenant)
  * **Respuesta (200 OK)**: Reservaciones de la tienda; **403 Forbidden** si falta tenant o el usuario no es staff.
* **`PATCH /{id}/estado`** (Staff con tenant)
  * **Payload**: `CambiarEstadoReservacionRequest`, con `estadoPago` y/o `estadoDespacho` opcionales.
  * **Respuesta (200 OK)**: Reservación actualizada; **404 Not Found** si no existe.

---

### Inventarios - `InventariosController`
Prefijo: `/api/v1/inventarios` (Las acciones verifican que se resuelva un tenant)

* **`GET /sucursal/{sucursalId}`**
  * **Respuesta (200 OK)**: Existencias de todos los productos en la sucursal indicada.
* **`PUT /ajuste`**
  * **Payload**: `{ "sucursalId": "string", "productoId": "string", "stock": 50 }`
  * **Respuesta (200 OK)**: Existencia de stock actualizada en el inventario físico de la sucursal.
  * **Error (400 Bad Request)**: No se pudo resolver el tenant.

---

### Dashboard y Reportes - `ReportesV1Controller`
Prefijo: `/api/v1/reportes` (Las acciones requieren tenant y usuario reconocido como Staff)

* **`POST /ejecutar-raw`** (Staff con tenant)
  * **Payload**: `{ "querySql": "SELECT ..." }`.
  * **Respuesta (200 OK)**: Resultado con filas (`rows`).
  * **Errores**: **400 Bad Request** para consultas inválidas; **403 Forbidden** si falta tenant o el solicitante no es staff; **500 Internal Server Error** ante otro error de ejecución.
* **`POST /guardar`** (Staff con tenant)
  * **Payload**: `{ "nombre": "string", "descripcion": "string|null", "querySql": "SELECT ..." }`.
  * **Respuesta (201 Created)**: Reporte guardado.
* **`GET /`** (Staff con tenant)
  * **Respuesta (200 OK)**: Reportes guardados para el tenant.
* **`GET /{id}/correr`** (Staff con tenant)
  * **Respuesta (200 OK)**: Resultado de ejecutar el reporte; **403 Forbidden** si falta tenant o el solicitante no es staff.

Los reportes analíticos del controlador legacy están bajo `/api/admin/reportes`: `GET /productos`, `GET /empleados`, `GET /stock-critico`, `GET /demanda` y `GET /metodos-pago`. Estos endpoints requieren autenticación y validan que el usuario pertenezca al panel administrativo.

---

### Pasarela de Pagos (Stripe) - `PagosController`
Prefijo: `/api/pagos`

* **`POST /create-intent`** (Cliente autenticado)
  * **Payload**: `{ "reservacionId": "string", "metodoPagoId": "string|null" }`.
  * **Respuesta (200 OK)**: `{ "clientSecret": "string", "reservacionId": "string", "montoCentavos": 0, "moneda": "string" }`.
  * **Errores**: **400 Bad Request** para una operación inválida; **401 Unauthorized** sin identidad válida; **403 Forbidden** si el token no pertenece a un cliente.
* **`GET /{paymentIntentId}/status`** (Autenticado)
  * **Respuesta (200 OK)**: `{ "paymentIntentId": "string", "status": "string", "reservacionId": "string", "montoCentavos": 0, "moneda": "string" }`.
  * **Errores**: **401 Unauthorized** para token inválido; **403 Forbidden** si no está autorizado a consultar ese pago; **404 Not Found** si no existe.
* **`POST /webhook`** (Público, sin JWT)
  * **Headers**: `Stripe-Signature` obligatorio.
  * **Payload**: Cuerpo JSON crudo enviado por Stripe.
  * **Respuesta (200 OK)**: `{ "recibido": true }` al procesar el evento.
  * **Errores**: **400 Bad Request** si falta la firma o no es válida; **500 Internal Server Error** ante un error interno al procesar el evento.

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