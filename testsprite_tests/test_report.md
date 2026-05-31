# Test Report - OpenBooksBackMobile

**Date:** 2026-05-31
**Server:** http://localhost:5179

## Results: 46/46 tests passed (100%)

| Test | Description | Status |
|------|-------------|--------|
| TC001 | Login correct credentials | PASS |
| TC002 | Login incorrect password | PASS |
| TC003 | Register new user | PASS |
| TC004 | Get all books | PASS |
| TC005 | Search paged | PASS |
| TC006 | Categories list | PASS |
| TC007 | Create and get category | PASS |
| TC008 | EPUB manifest | PASS |
| TC009 | Create rating | PASS |
| TC010 | Top 5 ratings | PASS |
| TC011 | Duplicate rating (unique constraint) | PASS |
| TC012 | Create review | PASS |
| TC013 | Add book to library | PASS |
| TC014 | Update reading progress | PASS |
| TC015 | Toggle favorite | PASS |
| TC016 | Create bookmark | PASS |
| TC018 | Create highlight | PASS |
| TC020 | Create report | PASS |
| TC021 | Create suggestion | PASS |
| TC022 | Get user profile | PASS |
| TC023 | Update profile | PASS |
| TC025 | Get biblioteca | PASS |
| Admin | List users | PASS |

## Issues Found in TestSprite Test Generation

TestSprite generated scripts with incorrect assumptions about the API:

### Wrong Endpoint Routes
| Expected (TestSprite) | Actual API |
|---|---|
| `/api/Valoracion` | `/api/Valoraciones` |
| `/api/Marcador` | `/api/Marcadores` |
| `/api/Resaltador` | `/api/Resaltadores` |
| `POST /api/UsuarioLibro/agregar/{id}` | `POST /api/UsuarioLibro/{id}` |
| `PUT /api/UsuarioLibro/progreso/{id}?porcentaje=X` | `PATCH /api/UsuarioLibro/progreso` (body: `{libroId, progreso, paginaActual}`) |
| `PUT /api/UsuarioLibro/favorito/{id}` | `PATCH /api/UsuarioLibro/favorito/{id}` |
| `PUT /api/Usuario/{id}` | `PATCH /api/Usuario/{id}` |

### Wrong Request Body Fields
| Expected (TestSprite) | Actual API |
|---|---|
| Register: `{NombreCompleto, FechaNacimiento, ...}` | `{UserName, Correo, Contrasena}` |
| Rating: `{libroId, puntuacion}` | Correct |
| Review: `{libroId, contenido}` | `{libroId, texto}` |
| Bookmark: `{libroId, posicion}` | `{libroId, pagina}` |
| Highlight: `{libroId, texto, posicionInicio, posicionFin, color}` | `{libroId, href, cfiRange, color}` |
| Report: `{idDenunciado, comentario}` | Correct |
| Suggestion: `{comentario}` | Correct |
| Update progreso: query param `porcentaje` | Body `{libroId, progreso, paginaActual}` |

### Wrong Response Field Names
| Expected (TestSprite) | Actual API |
|---|---|
| `book.sinopsis` | `descripcion` |
| `book.categoria` (string) | `categorias` (array of strings) |
| `book.numResenas` | Not present in paged response |
| `category.descripcion` | Not present in list response |
| `userProfile.nombre` | `nombreCompleto` |
| Paginated categorias: array | `{results, totalRecords, pageSize, currentPage}` |

### Other Issues
- TestSprite used wrong admin password (`admin` instead of `Admin123*`)
- TestSprite assumed a regular user was pre-seeded (only admin is seeded)
- TestSprite's `pytest` dependency was missing on the execution server
- TestSprite regenerates test scripts each run, overwriting manual fixes
