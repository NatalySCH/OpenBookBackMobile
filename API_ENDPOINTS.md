# Documentación de API - OpenBookBackMobile

> Base URL: `http://localhost:{puerto}`  
> Autenticación: JWT Bearer Token (enviar en header `Authorization: Bearer {token}`)

---

## Tabla de Contenidos
1. [Auth](#1-auth)
2. [Usuario](#2-usuario)
3. [Libro](#3-libro)
4. [Categorias](#4-categorias)
5. [Epub](#5-epub)
6. [Marcadores](#6-marcadores)
7. [Resaltadores](#7-resaltadores)
8. [UsuarioLibro](#8-usuariolibro)
9. [Resena](#9-resena)
10. [Valoraciones](#10-valoraciones)
11. [Sugerencia](#11-sugerencia)
12. [Denuncia](#12-denuncia)

---

## 1. Auth
**Ruta Base:** `api/Auth`

### POST /api/Auth/login
**Descripción:** Inicia sesión de usuario, devuelve JWT token

**Autenticación:** ❌ No requiere

**Request Body (JSON):**
```json
{
  "Correo": "string",
  "Contrasena": "string"
}
```

**Response 200 OK:**
```json
{
  "token": "string",
  "username": "string",
  "correo": "string",
  "fotoPerfilUrl": "string | null"
}
```

---

### POST /api/Auth/register
**Descripción:** Registra un nuevo usuario con rol "Usuario"

**Autenticación:** ❌ No requiere

**Request Body (JSON):**
```json
{
  "UserName": "string",
  "Correo": "string",
  "Contrasena": "string"
}
```

**Response 200 OK:**
```json
{
  "mensaje": "string"
}
```

---

## 2. Usuario
**Ruta Base:** `api/Usuario`

### GET /api/Usuario
**Descripción:** Lista paginada de usuarios

**Autenticación:** ✅ Requiere JWT (`[Authorize]`)

**Query Params:**
| Parametro | Tipo | Default | Descripcion |
|-----------|------|---------|-------------|
| pageNumber | int | 1 | Numero de pagina |
| pageSize | int | 10 | Cantidad por pagina |

**Response 200 OK:** `PagedResult<UsuarioResponseDto>`
```json
{
  "Items": [
    {
      "Id": "string",
      "UserName": "string",
      "Email": "string",
      "NombreCompleto": "string",
      "FotoPerfilUrl": "string | null",
      "NombreRol": "string"
    }
  ],
  "TotalCount": 0,
  "PageNumber": 1,
  "PageSize": 10,
  "TotalPages": 1
}
```

---

### GET /api/Usuario/{id}
**Descripción:** Obtiene un usuario por ID

**Autenticación:** ✅ Requiere JWT (`[Authorize]`)

**Route Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| id | string | ID del usuario |

**Response 200 OK:** `UsuarioResponseDto`
```json
{
  "Id": "string",
  "UserName": "string",
  "Email": "string",
  "NombreCompleto": "string",
  "FotoPerfilUrl": "string | null",
  "NombreRol": "string"
}
```

**Response 404 Not Found:** Usuario no existe

---

### POST /api/Usuario
**Descripción:** Crea un nuevo usuario

**Autenticación:** ❌ No requiere

**Request Body (JSON):** `UsuarioCreateDto`
```json
{
  "UserName": "string",
  "Email": "string",
  "NombreCompleto": "string",
  "Contraseña": "string",
  "FotoPerfilUrl": "string | null"
}
```

**Response 201 Created:** `UsuarioResponseDto`

---

### POST /api/Usuario/upload-foto
**Descripción:** Sube una foto de perfil

**Autenticación:** ❌ No requiere

**Request:** `multipart/form-data`
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| archivo | IFormFile | Archivo de imagen |

**Response 200 OK:**
```json
{
  "url": "string"
}
```

---

### PATCH /api/Usuario/{id}
**Descripción:** Actualiza parcialmente un usuario

**Autenticación:** ✅ Requiere JWT (`[Authorize]`)

**Route Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| id | string | ID del usuario |

**Request Body (JSON):** `UsuarioUpdateDto` (todos los campos son opcionales)
```json
{
  "UserName": "string | null",
  "Email": "string | null",
  "NombreCompleto": "string | null",
  "Contraseña": "string | null",
  "FotoPerfilUrl": "string | null"
}
```

**Response 204 No Content:** Actualizacion exitosa  
**Response 404 Not Found:** Usuario no existe

---

### DELETE /api/Usuario/{id}
**Descripción:** Elimina un usuario

**Autenticación:** ✅ Requiere JWT (`[Authorize]`)

**Route Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| id | string | ID del usuario |

**Response 204 No Content:** Eliminacion exitosa  
**Response 404 Not Found:** Usuario no existe

---

### POST /api/Usuario/solicitar-recuperacion
**Descripción:** Solicita recuperacion de contrasena, devuelve token de reset

**Autenticación:** ❌ No requiere

**Request Body (JSON):** `SolicitarRecuperacionDto`
```json
{
  "Correo": "string"
}
```

**Response 200 OK:**
```json
{
  "token": "string"
}
```

**Response 404 Not Found:** Correo no registrado

---

### POST /api/Usuario/reset-password
**Descripción:** Resetea la contrasena usando token

**Autenticación:** ❌ No requiere

**Request Body (JSON):** `ResetPasswordDto`
```json
{
  "Email": "string",
  "Token": "string",
  "NuevaContraseña": "string"
}
```

**Response 200 OK:**
```
"Contraseña actualizada"
```

---

## 3. Libro
**Ruta Base:** `api/Libro`

### GET /api/Libro
**Descripción:** Obtiene todo el catalogo de libros

**Autenticación:** ❌ No requiere

**Response 200 OK:** `List<LibroResponseDto>`
```json
[
  {
    "Id": 0,
    "Titulo": "string",
    "Autor": "string",
    "Descripcion": "string | null",
    "PortadaUrl": "string | null",
    "ArchivoUrl": "string",
    "EsPublico": false,
    "FechaCreacion": "2026-05-10T00:00:00Z",
    "UsuarioCreadorId": "string | null",
    "PromedioValoracion": 0.0,
    "TotalValoraciones": 0,
    "Categorias": ["string"]
  }
]
```

---

### POST /api/Libro/upload-libro
**Descripción:** Sube un archivo de libro (epub, etc.)

**Autenticación:** ❌ No requiere

**Request:** `multipart/form-data`
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| archivo | IFormFile | Archivo del libro (.epub) |

**Response 200 OK:**
```json
{
  "url": "string",
  "fileName": "string"
}
```

---

### POST /api/Libro/upload-portada
**Descripción:** Sube una imagen de portada

**Autenticación:** ❌ No requiere

**Request:** `multipart/form-data`
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| archivo | IFormFile | Archivo de imagen |

**Response 200 OK:**
```json
{
  "url": "string"
}
```

---

### POST /api/Libro
**Descripción:** Crea un nuevo libro (el usuario autenticado es el creador)

**Autenticación:** ✅ Requiere JWT (`[Authorize]`)

**Request:** `multipart/form-data` con `LibroCreateDto`
```json
{
  "Titulo": "string",
  "Autor": "string",
  "Descripcion": "string | null",
  "ArchivoUrl": "string",
  "PortadaUrl": "string | null",
  "CategoriaIds": [1, 2, 3],
  "EsPublico": false
}
```

**Response 200 OK:** `LibroResponseDto`

---

### GET /api/Libro/paged
**Descripción:** Busqueda paginada de libros con filtros

**Autenticación:** ❌ No requiere

**Query Params:**
| Parametro | Tipo | Default | Descripcion |
|-----------|------|---------|-------------|
| query | string | null | Texto para buscar en titulo/autor |
| page | int | 1 | Numero de pagina |
| pageSize | int | 10 | Cantidad por pagina |
| autor | string | null | Filtrar por autor |
| categoriaId | int | null | Filtrar por categoria ID |

**Response 200 OK:**
```json
{
  "Page": 1,
  "PageSize": 10,
  "Total": 0,
  "TotalPages": 1,
  "Data": [/* LibroResponseDto */]
}
```

---

### GET /api/Libro/{id}/descargar
**Descripción:** Descarga el archivo EPUB de un libro

**Autenticación:** ✅ Requiere JWT (`[Authorize]`)

**Route Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| id | int | ID del libro |

**Response 200 OK:** Archivo `application/epub+zip`  
**Response 404 Not Found:** Libro no existe

---

### POST /api/Libro/{id}/categorias
**Descripción:** Asigna categorias a un libro

**Autenticación:** ✅ Requiere JWT (`[Authorize]`)

**Route Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| id | int | ID del libro |

**Request Body (JSON):** `List<int>` (IDs de categorias)
```json
[1, 2, 3]
```

**Response 200 OK:** Exito

---

### DELETE /api/Libro/{id}
**Descripción:** Elimina un libro

**Autenticación:** ❌ No requiere

**Route Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| id | int | ID del libro |

**Response 204 No Content:** Eliminacion exitosa  
**Response 404 Not Found:** Libro no existe

---

## 4. Categorias
**Ruta Base:** `api/Categorias`

### GET /api/Categorias
**Descripción:** Lista paginada de categorias

**Autenticación:** ❌ No requiere

**Query Params:**
| Parametro | Tipo | Default | Descripcion |
|-----------|------|---------|-------------|
| pageNumber | int | 1 | Numero de pagina |
| pageSize | int | 10 | Cantidad por pagina |

**Response 200 OK:** `PagedResult<CategoriaResponseDto>`
```json
{
  "Items": [
    {
      "Id": 0,
      "Nombre": "string",
      "TotalLibros": 0
    }
  ],
  "TotalCount": 0,
  "PageNumber": 1,
  "PageSize": 10,
  "TotalPages": 1
}
```

---

### GET /api/Categorias/{id}
**Descripción:** Obtiene una categoria por ID

**Autenticación:** ❌ No requiere

**Route Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| id | int | ID de la categoria |

**Response 200 OK:** `CategoriaResponseDto`
```json
{
  "Id": 0,
  "Nombre": "string",
  "TotalLibros": 0
}
```

**Response 404 Not Found:** Categoria no existe

---

### POST /api/Categorias
**Descripción:** Crea una nueva categoria

**Autenticación:** ❌ No requiere

**Request Body (JSON):** `CategoriaCreateDto`
```json
{
  "Nombre": "string"
}
```

**Response 201 Created:** `CategoriaResponseDto`

---

### PATCH /api/Categorias/{id}
**Descripción:** Actualiza parcialmente una categoria

**Autenticación:** ❌ No requiere

**Route Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| id | int | ID de la categoria |

**Request Body (JSON):** `CategoriaUpdateDto`
```json
{
  "Nombre": "string | null"
}
```

**Response 204 No Content:** Actualizacion exitosa  
**Response 404 Not Found:** Categoria no existe

---

### DELETE /api/Categorias/{id}
**Descripción:** Elimina una categoria

**Autenticación:** ❌ No requiere

**Route Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| id | int | ID de la categoria |

**Response 204 No Content:** Eliminacion exitosa  
**Response 404 Not Found:** Categoria no existe

---

## 5. Epub
**Ruta Base:** `api/epub`

### GET /api/epub/{libroId}/manifest
**Descripción:** Obtiene el manifiesto de un libro EPUB (tabla de contenidos, recursos, orden de lectura)

**Autenticación:** ❌ No requiere

**Route Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| libroId | int | ID del libro |

**Response 200 OK:** `BookManifestDto`
```json
{
  "Id": 0,
  "Titulo": "string",
  "Autor": "string",
  "ReadingOrder": [
    {
      "Href": "string",
      "Title": "string"
    }
  ],
  "Resources": [
    {
      "Href": "string",
      "MediaType": "string"
    }
  ],
  "Toc": [
    {
      "Title": "string",
      "Href": "string",
      "Level": 0
    }
  ]
}
```

**Response 404 Not Found:** Libro no existe

---

### GET /api/epub/{libroId}/resource
**Descripción:** Obtiene un recurso especifico del EPUB (ej: un capitulo HTML, una imagen)

**Autenticación:** ❌ No requiere

**Route Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| libroId | int | ID del libro |

**Query Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| path | string | Ruta del recurso dentro del EPUB |

**Response 200 OK:** Archivo binario con su `Content-Type` correspondiente  
**Response 404 Not Found:** Recurso no existe

---

## 6. Marcadores
**Ruta Base:** `api/Marcadores`  
**Autenticación:** ✅ Todo el controlador requiere JWT (`[Authorize]`)

### POST /api/Marcadores
**Descripción:** Crea un nuevo marcador de pagina

**Request:** `multipart/form-data` con `MarcadorCreateDto`
```json
{
  "LibroId": 0,
  "Pagina": 0
}
```

**Response 200 OK:** `MarcadorResponseDto`
```json
{
  "Id": 0,
  "LibroId": 0,
  "TituloLibro": "string",
  "Pagina": 0,
  "Fecha": "2026-05-10T00:00:00Z"
}
```

---

### GET /api/Marcadores/usuario
**Descripción:** Obtiene todos los marcadores del usuario autenticado

**Response 200 OK:** `List<MarcadorResponseDto>`

---

### GET /api/Marcadores/libro/{libroId}
**Descripción:** Obtiene marcadores de un libro especifico para el usuario autenticado

**Route Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| libroId | int | ID del libro |

**Response 200 OK:** `List<MarcadorResponseDto>`

---

### PUT /api/Marcadores/{id}
**Descripción:** Actualiza un marcador

**Route Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| id | int | ID del marcador |

**Request:** `multipart/form-data` con `MarcadorUpdateDto`
```json
{
  "Pagina": 0
}
```

**Response 204 No Content:** Actualizacion exitosa  
**Response 404 Not Found:** Marcador no existe

---

### DELETE /api/Marcadores/{id}
**Descripción:** Elimina un marcador

**Route Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| id | int | ID del marcador |

**Response 204 No Content:** Eliminacion exitosa  
**Response 404 Not Found:** Marcador no existe

---

## 7. Resaltadores
**Ruta Base:** `api/Resaltadores`  
**Autenticación:** ✅ Todo el controlador requiere JWT (`[Authorize]`)

### POST /api/Resaltadores
**Descripción:** Crea un nuevo resaltado/subrayado en un libro

**Request:** `multipart/form-data` con `ResaltadorCreateDto`
```json
{
  "LibroId": 0,
  "Href": "string",
  "CfiRange": "string",
  "Color": "#FFFF00"
}
```

**Response 200 OK:** `ResaltadorResponseDto`
```json
{
  "Id": 0,
  "LibroId": 0,
  "TituloLibro": "string",
  "Href": "string",
  "CfiRange": "string",
  "Color": "string"
}
```

---

### GET /api/Resaltadores/usuario
**Descripción:** Obtiene todos los resaltados del usuario autenticado

**Response 200 OK:** `List<ResaltadorResponseDto>`

---

### GET /api/Resaltadores/libro/{libroId}
**Descripción:** Obtiene resaltados de un libro especifico para el usuario

**Route Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| libroId | int | ID del libro |

**Response 200 OK:** `List<ResaltadorResponseDto>`

---

### PUT /api/Resaltadores/{id}
**Descripción:** Actualiza un resaltado

**Route Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| id | int | ID del resaltado |

**Request:** `multipart/form-data` con `ResaltadorUpdateDto`
```json
{
  "CfiRange": "string",
  "Color": "string"
}
```

**Response 204 No Content:** Actualizacion exitosa  
**Response 404 Not Found:** Resaltado no existe

---

### DELETE /api/Resaltadores/{id}
**Descripción:** Elimina un resaltado

**Route Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| id | int | ID del resaltado |

**Response 204 No Content:** Eliminacion exitosa  
**Response 404 Not Found:** Resaltado no existe

---

## 8. UsuarioLibro
**Ruta Base:** `api/UsuarioLibro`  
**Autenticación:** ✅ Todo el controlador requiere JWT (`[Authorize]`)

### GET /api/UsuarioLibro/biblioteca
**Descripción:** Obtiene la biblioteca personal del usuario autenticado

**Response 200 OK:** `List<UsuarioLibroDto>`
```json
[
  {
    "LibroId": 0,
    "Titulo": "string",
    "Autor": "string",
    "PortadaUrl": "string | null",
    "Progreso": 0.0,
    "PaginaActual": 0,
    "EsFavorito": false,
    "UltimaLectura": "2026-05-10T00:00:00Z | null"
  }
]
```

---

### POST /api/UsuarioLibro/{libroId}
**Descripción:** Agrega un libro a la biblioteca del usuario

**Route Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| libroId | int | ID del libro |

**Response 200 OK:**
```
"Libro agregado"
```

**Response 400 Bad Request:** Libro ya esta en la biblioteca

---

### DELETE /api/UsuarioLibro/{libroId}
**Descripción:** Elimina un libro de la biblioteca del usuario

**Route Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| libroId | int | ID del libro |

**Response 204 No Content:** Eliminacion exitosa  
**Response 404 Not Found:** Libro no esta en la biblioteca

---

### PATCH /api/UsuarioLibro/progreso
**Descripción:** Actualiza el progreso de lectura de un libro

**Request:** `multipart/form-data` con `UpdateProgresoDto`
```json
{
  "LibroId": 0,
  "Progreso": 0.0,
  "PaginaActual": 0
}
```

**Response 204 No Content:** Actualizacion exitosa  
**Response 404 Not Found:** Libro no esta en la biblioteca

---

### PATCH /api/UsuarioLibro/favorito/{libroId}
**Descripción:** Marca/desmarca un libro como favorito (toggle)

**Route Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| libroId | int | ID del libro |

**Response 204 No Content:** Actualizacion exitosa  
**Response 404 Not Found:** Libro no esta en la biblioteca

---

## 9. Resena
**Ruta Base:** `api/Resena`

### POST /api/Resena
**Descripción:** Crea una resena para un libro

**Autenticación:** ✅ Requiere JWT (`[Authorize]`)

**Request Body (JSON):** `ResenaCreateDto`
```json
{
  "LibroId": 0,
  "Texto": "string"
}
```

**Response 200 OK:** `ResenaResponseDto`
```json
{
  "Id": 0,
  "LibroId": 0,
  "UsuarioId": "string",
  "NombreUsuario": "string",
  "Texto": "string",
  "Fecha": "2026-05-10T00:00:00Z"
}
```

---

### PUT /api/Resena/{id}
**Descripción:** Actualiza una resena existente (solo el autor)

**Autenticación:** ✅ Requiere JWT (`[Authorize]`)

**Route Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| id | int | ID de la resena |

**Request Body (JSON):** `ResenaUpdateDto`
```json
{
  "Texto": "string"
}
```

**Response 200 OK:**
```
"Reseña actualizada"
```

---

### DELETE /api/Resena/{id}
**Descripción:** Elimina una resena (solo el autor)

**Autenticación:** ✅ Requiere JWT (`[Authorize]`)

**Route Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| id | int | ID de la resena |

**Response 200 OK:**
```
"Reseña eliminada"
```

**Response 404 Not Found:** Resena no existe

---

### GET /api/Resena/libro/{libroId}
**Descripción:** Obtiene todas las resenas de un libro

**Autenticación:** ❌ No requiere

**Route Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| libroId | int | ID del libro |

**Response 200 OK:** `List<ResenaResponseDto>`

---

## 10. Valoraciones
**Ruta Base:** `api/Valoraciones`

### POST /api/Valoraciones
**Descripción:** Crea una valoracion (puntuacion 1-5) para un libro

**Autenticación:** ✅ Requiere JWT (`[Authorize]`)

**Request Body (JSON):** `ValoracionCreateDto`
```json
{
  "LibroId": 0,
  "Puntuacion": 0
}
```

**Response 200 OK:** `ValoracionResponseDto`
```json
{
  "Id": 0,
  "LibroId": 0,
  "UsuarioId": "string",
  "Puntuacion": 0,
  "Fecha": "2026-05-10T00:00:00Z"
}
```

---

### PUT /api/Valoraciones/{libroId}
**Descripción:** Actualiza la valoracion de un libro

**Autenticación:** ✅ Requiere JWT (`[Authorize]`)

**Route Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| libroId | int | ID del libro |

**Request Body (JSON):** `ValoracionUpdateDto`
```json
{
  "Puntuacion": 0
}
```

**Response 200 OK:**
```
"Actualizado correctamente."
```

---

### DELETE /api/Valoraciones/{libroId}
**Descripción:** Elimina la valoracion de un libro

**Autenticación:** ✅ Requiere JWT (`[Authorize]`)

**Route Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| libroId | int | ID del libro |

**Response 200 OK:**
```
"Eliminada correctamente."
```

**Response 404 Not Found:** Valoracion no existe

---

### GET /api/Valoraciones/libro/{libroId}
**Descripción:** Obtiene todas las valoraciones de un libro

**Autenticación:** ❌ No requiere

**Route Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| libroId | int | ID del libro |

**Response 200 OK:** `List<ValoracionResponseDto>`

---

### GET /api/Valoraciones/top5
**Descripción:** Obtiene las 5 mejores valoraciones

**Autenticación:** ❌ No requiere

**Response 200 OK:** `List<ValoracionResponseDto>`

---

## 11. Sugerencia
**Ruta Base:** `api/Sugerencia`

### POST /api/Sugerencia
**Descripción:** Envia una sugerencia (usuario autenticado)

**Autenticación:** ✅ Requiere JWT (`[Authorize]`)

**Request Body (JSON):** `SugerenciaCreateDto`
```json
{
  "Comentario": "string"
}
```

**Response 200 OK:** `SugerenciaResponseDto`
```json
{
  "Id": 0,
  "UsuarioId": "string",
  "NombreUsuario": "string",
  "Comentario": "string"
}
```

**Response 400 Bad Request:** Error de validacion

---

### GET /api/Sugerencia
**Descripción:** Lista paginada de sugerencias (solo admin)

**Autenticación:** ✅ Requiere JWT + Rol `Administrador` (`[Authorize(Roles = "Administrador")]`)

**Query Params:**
| Parametro | Tipo | Default | Descripcion |
|-----------|------|---------|-------------|
| pagina | int | 1 | Numero de pagina |
| tamanoPagina | int | 10 | Cantidad por pagina |

**Response 200 OK:**
```json
{
  "PaginaActual": 1,
  "TamanoPagina": 10,
  "TotalRegistros": 0,
  "TotalPaginas": 1,
  "Datos": [/* SugerenciaResponseDto */]
}
```

---

### DELETE /api/Sugerencia/{id}
**Descripción:** Elimina una sugerencia (solo admin)

**Autenticación:** ✅ Requiere JWT + Rol `Administrador` (`[Authorize(Roles = "Administrador")]`)

**Route Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| id | int | ID de la sugerencia |

**Response 204 No Content:** Eliminacion exitosa  
**Response 404 Not Found:** Sugerencia no existe

---

## 12. Denuncia
**Ruta Base:** `api/Denuncia`

### POST /api/Denuncia
**Descripción:** Crea una denuncia contra otro usuario (no permite autodenuncia)

**Autenticación:** ✅ Requiere JWT (`[Authorize]`)

**Request Body (JSON):** `DenunciaCreateDto`
```json
{
  "IdDenunciado": "string",
  "Comentario": "string"
}
```

**Response 201 Created:** `DenunciaResponseDto`
```json
{
  "Id": 0,
  "IdDenunciante": "string",
  "NombreDenunciante": "string",
  "IdDenunciado": "string",
  "NombreDenunciado": "string",
  "Comentario": "string"
}
```

---

### GET /api/Denuncia
**Descripción:** Lista paginada de denuncias (solo admin)

**Autenticación:** ✅ Requiere JWT + Rol `Administrador` (`[Authorize(Roles = "Administrador")]`)

**Query Params:**
| Parametro | Tipo | Default | Descripcion |
|-----------|------|---------|-------------|
| pagina | int | 1 | Numero de pagina |
| tamanoPagina | int | 10 | Cantidad por pagina |

**Response 200 OK:**
```json
{
  "PaginaActual": 1,
  "TamanoPagina": 10,
  "TotalRegistros": 0,
  "TotalPaginas": 1,
  "Datos": [/* DenunciaResponseDto */]
}
```

---

### DELETE /api/Denuncia/{id}
**Descripción:** Elimina una denuncia (solo admin)

**Autenticación:** ✅ Requiere JWT + Rol `Administrador` (`[Authorize(Roles = "Administrador")]`)

**Route Params:**
| Parametro | Tipo | Descripcion |
|-----------|------|-------------|
| id | int | ID de la denuncia |

**Response 204 No Content:** Eliminacion exitosa  
**Response 404 Not Found:** Denuncia no existe

---

## Resumen Estadistico

| Categoria | Endpoints | Autenticados | Requieren Admin | Publicos |
|-----------|-----------|-------------|-----------------|----------|
| Auth | 2 | 0 | 0 | 2 |
| Usuario | 8 | 4 | 0 | 4 |
| Libro | 9 | 3 | 0 | 6 |
| Categorias | 5 | 0 | 0 | 5 |
| Epub | 2 | 0 | 0 | 2 |
| Marcadores | 5 | 5 | 0 | 0 |
| Resaltadores | 5 | 5 | 0 | 0 |
| UsuarioLibro | 5 | 5 | 0 | 0 |
| Resena | 4 | 3 | 0 | 1 |
| Valoraciones | 5 | 3 | 0 | 2 |
| Sugerencia | 3 | 3 | 2 | 0 |
| Denuncia | 3 | 3 | 2 | 0 |
| **TOTAL** | **56** | **34** | **4** | **22** |

---

## Informacion Tecnica

- **Framework:** ASP.NET Core 8
- **Autenticacion:** JWT Bearer Token
- **Base de Datos:** PostgreSQL + Entity Framework Core + Identity
- **Roles:** `Administrador`, `Usuario`
- **Documentacion:** Swagger/OpenAPI disponible en `/swagger` (entorno desarrollo)
