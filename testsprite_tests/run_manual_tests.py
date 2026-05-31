import requests, sys, time, uuid, os

BASE_URL = "http://localhost:5179"
TIMEOUT = 15
passed = 0
failed = 0
results = []

os.environ["PYTHONIOENCODING"] = "utf-8"

def check(condition, msg):
    global passed, failed
    if condition:
        passed += 1
        results.append(("PASS", msg))
        print(f"  [PASS] {msg}")
    else:
        failed += 1
        results.append(("FAIL", msg))
        print(f"  [FAIL] {msg}")

# --- Auth: Login as admin and register test user ---
print("\n=== TC001: Login correct credentials ===")
ADMIN = {"Correo": "admin@gmail.com", "Contrasena": "Admin123*"}
r = requests.post(f"{BASE_URL}/api/Auth/login", json=ADMIN, timeout=TIMEOUT)
check(r.status_code == 200, f"Login admin 200 (got {r.status_code})")
if r.ok:
    data = r.json()
    check("token" in data and len(data["token"]) > 0, "token present")
    admin_token = data["token"]

print("\n=== TC002: Login incorrect password ===")
r = requests.post(f"{BASE_URL}/api/Auth/login",
                  json={"Correo": "admin@gmail.com", "Contrasena": "wrong"}, timeout=TIMEOUT)
check(r.status_code == 401, f"Wrong password 401 (got {r.status_code})")

print("\n=== TC003: Register new user ===")
unique_email = f"reg_{uuid.uuid4().hex[:8]}@test.com"
r = requests.post(f"{BASE_URL}/api/Auth/register",
                  json={"Correo": unique_email, "Contrasena": "NewUser123*",
                        "UserName": f"user_{uuid.uuid4().hex[:6]}"}, timeout=TIMEOUT)
check(r.status_code == 200, f"Register 200 (got {r.status_code})")
# Login as the new user to get a token for user-scoped tests
USER = {"Correo": unique_email, "Contrasena": "NewUser123*"}
r = requests.post(f"{BASE_URL}/api/Auth/login", json=USER, timeout=TIMEOUT)
user_token = r.json().get("token") if r.ok else None
user_headers = {"Authorization": f"Bearer {user_token}"} if user_token else {}
admin_headers = {"Authorization": f"Bearer {admin_token}"} if admin_token else {}

# --- Catalog ---
print("\n=== TC004: Get all books ===")
r = requests.get(f"{BASE_URL}/api/Libro", timeout=TIMEOUT)
check(r.status_code == 200, f"Libros 200 (got {r.status_code})")
if r.ok:
    libros = r.json()
    check(isinstance(libros, list) and len(libros) > 0, f"List with {len(libros)} books")

print("\n=== TC005: Search paged ===")
r = requests.get(f"{BASE_URL}/api/Libro/paged?page=1&pageSize=10", timeout=TIMEOUT)
check(r.status_code == 200, f"Paged 200 (got {r.status_code})")
if r.ok:
    p = r.json()
    check("page" in p and "data" in p, "Has page/data fields")
    check(isinstance(p.get("data"), list), "data is list")
    if p.get("data"):
        item = p["data"][0]
        for f in ["id", "titulo", "autor", "descripcion", "categorias", "portadaUrl"]:
            check(f in item, f"Field '{f}' in book item")

# --- Categories ---
print("\n=== TC006: Categories list ===")
r = requests.get(f"{BASE_URL}/api/Categorias", timeout=TIMEOUT)
check(r.status_code == 200, f"Categorias 200 (got {r.status_code})")
if r.ok:
    cats_data = r.json()
    check("results" in cats_data, "Categories has 'results' field")
    check(isinstance(cats_data["results"], list) and len(cats_data["results"]) > 0,
          f"List with {len(cats_data['results'])} categories")
    for f in ["id", "nombre", "totalLibros"]:
        check(f in cats_data["results"][0], f"Field '{f}' in category")

print("\n=== TC007: Create and get category ===")
cat_name = f"TCat_{uuid.uuid4().hex[:6]}"
r = requests.post(f"{BASE_URL}/api/Categorias",
                  json={"nombre": cat_name, "descripcion": "Test"}, timeout=TIMEOUT)
check(r.status_code in [200, 201], f"Create category {r.status_code}")
r2 = requests.get(f"{BASE_URL}/api/Categorias", timeout=TIMEOUT)
check(r2.ok, "List categories ok")
if r2.ok:
    names = [c.get("nombre") for c in r2.json().get("results", [])]
    check(cat_name in names, "New category found in list")

# --- EPUB ---
print("\n=== TC008: EPUB manifest ===")
r = requests.get(f"{BASE_URL}/api/Libro/paged?page=1&pageSize=1", timeout=TIMEOUT)
if r.ok and r.json().get("data"):
    libro_id = r.json()["data"][0].get("id")
    r2 = requests.get(f"{BASE_URL}/api/Libro/{libro_id}/epub", timeout=TIMEOUT)
    check(r2.status_code in [200, 404],
          f"EPUB manifest {r2.status_code} (404 = no EPUB file)")

# --- Ratings (ValoracionesController -> /api/Valoraciones) ---
print("\n=== TC009: Create rating ===")
r = requests.get(f"{BASE_URL}/api/Libro/paged?page=1&pageSize=1", timeout=TIMEOUT)
if r.ok and r.json().get("data"):
    lid = r.json()["data"][0].get("id")
    r2 = requests.post(f"{BASE_URL}/api/Valoraciones",
                       json={"libroId": lid, "puntuacion": 4},
                       headers=user_headers, timeout=TIMEOUT)
    check(r2.status_code in [200, 201, 400],
          f"Create rating {r2.status_code} (400 = duplicate)")

print("\n=== TC010: Top 5 ratings ===")
r = requests.get(f"{BASE_URL}/api/Valoraciones/top5", timeout=TIMEOUT)
check(r.status_code == 200, f"Top5 200 (got {r.status_code})")
if r.ok:
    data = r.json()
    check(isinstance(data, list), "Top5 is list")
    if data:
        check("libroId" in data[0], "Item has 'libroId'")
        check("titulo" in data[0], "Item has 'titulo'")
        check("promedio" in data[0], "Item has 'promedio'")
        check("totalValoraciones" in data[0], "Item has 'totalValoraciones'")

print("\n=== TC011: Duplicate rating (unique constraint) ===")
r = requests.get(f"{BASE_URL}/api/Libro/paged?page=1&pageSize=1", timeout=TIMEOUT)
if r.ok and r.json().get("data"):
    lid = r.json()["data"][0].get("id")
    r2 = requests.post(f"{BASE_URL}/api/Valoraciones",
                       json={"libroId": lid, "puntuacion": 3},
                       headers=user_headers, timeout=TIMEOUT)
    if r2.status_code in [200, 201]:
        r3 = requests.post(f"{BASE_URL}/api/Valoraciones",
                           json={"libroId": lid, "puntuacion": 5},
                           headers=user_headers, timeout=TIMEOUT)
        check(r3.status_code in [400, 409],
              f"Duplicate rating {r3.status_code} (expect 400 or 409)")

# --- Reviews ---
print("\n=== TC012: Create review ===")
r = requests.get(f"{BASE_URL}/api/Libro/paged?page=1&pageSize=1", timeout=TIMEOUT)
if r.ok and r.json().get("data"):
    lid = r.json()["data"][0].get("id")
    r2 = requests.post(f"{BASE_URL}/api/Resena",
                       json={"libroId": lid, "texto": "Great book!"},
                       headers=user_headers, timeout=TIMEOUT)
    check(r2.status_code in [200, 201], f"Create review {r2.status_code}")

# --- User Library ---
print("\n=== TC013: Add book to library ===")
r = requests.get(f"{BASE_URL}/api/Libro/paged?page=1&pageSize=1", timeout=TIMEOUT)
if r.ok and r.json().get("data"):
    lid = r.json()["data"][0].get("id")
    r2 = requests.post(f"{BASE_URL}/api/UsuarioLibro/{lid}",
                       headers=user_headers, timeout=TIMEOUT)
    check(r2.status_code in [200, 201], f"Add to library {r2.status_code}")

print("\n=== TC014: Update reading progress ===")
if user_headers:
    r = requests.patch(f"{BASE_URL}/api/UsuarioLibro/progreso",
                       json={"libroId": 1, "progreso": 0.5, "paginaActual": 15},
                       headers=user_headers, timeout=TIMEOUT)
    check(r.status_code in [200, 204, 404],
          f"Update progress {r.status_code} (404 = libroId=1 not in library)")

print("\n=== TC015: Toggle favorite ===")
if user_headers:
    r = requests.patch(f"{BASE_URL}/api/UsuarioLibro/favorito/1",
                       headers=user_headers, timeout=TIMEOUT)
    check(r.status_code in [200, 204, 404],
          f"Toggle favorite {r.status_code} (404 = libroId=1 not in library)")

print("\n=== TC025: Get biblioteca ===")
if user_headers:
    r = requests.get(f"{BASE_URL}/api/UsuarioLibro/biblioteca",
                     headers=user_headers, timeout=TIMEOUT)
    check(r.status_code == 200, f"Biblioteca 200 (got {r.status_code})")
    if r.ok:
        b = r.json()
        check(isinstance(b, list), "Biblioteca is list")

# --- Bookmarks (MarcadoresController -> /api/Marcadores) ---
print("\n=== TC016: Create bookmark ===")
if user_headers:
    r = requests.post(f"{BASE_URL}/api/Marcadores",
                      json={"libroId": 1, "pagina": 10},
                      headers=user_headers, timeout=TIMEOUT)
    check(r.status_code in [200, 201, 404],
          f"Create bookmark {r.status_code}")

# --- Highlights (ResaltadoresController -> /api/Resaltadores) ---
print("\n=== TC018: Create highlight ===")
if user_headers:
    r = requests.post(f"{BASE_URL}/api/Resaltadores",
                      json={"libroId": 1, "href": "text.html", "cfiRange": "/6/4[chap]!/4/2/1:0",
                            "color": "#FFFF00"},
                      headers=user_headers, timeout=TIMEOUT)
    check(r.status_code in [200, 201, 404],
          f"Create highlight {r.status_code}")

# --- Reports ---
print("\n=== TC020: Create report ===")
if user_token:
    r_users = requests.get(f"{BASE_URL}/api/Usuario",
                           headers=admin_headers, timeout=TIMEOUT)
    target_id = "unknown-id"
    if r_users.ok:
        users_data = r_users.json()
        if isinstance(users_data, dict) and users_data.get("results"):
            for u in users_data["results"]:
                if u.get("email") != unique_email:
                    target_id = u.get("id")
                    break
    r = requests.post(f"{BASE_URL}/api/Denuncia",
                      json={"idDenunciado": target_id, "comentario": "Test report"},
                      headers=user_headers, timeout=TIMEOUT)
    check(r.status_code in [200, 201, 400],
          f"Create report {r.status_code}")

# --- Suggestions ---
print("\n=== TC021: Create suggestion ===")
if user_headers:
    r = requests.post(f"{BASE_URL}/api/Sugerencia",
                      json={"comentario": "Please add more sci-fi books"},
                      headers=user_headers, timeout=TIMEOUT)
    check(r.status_code in [200, 201], f"Create suggestion {r.status_code}")

# --- User Profile ---
print("\n=== TC022: Get user by ID (profile) ===")
if user_headers:
    # Get user ID from the /Usuario list (or we can use GET /api/Usuario with filters)
    r_list = requests.get(f"{BASE_URL}/api/Usuario?pageNumber=1&pageSize=10",
                          headers=admin_headers, timeout=TIMEOUT)
    if r_list.ok and r_list.json().get("results"):
        for u in r_list.json()["results"]:
            if u.get("email") == unique_email:
                uid = u.get("id")
                r = requests.get(f"{BASE_URL}/api/Usuario/{uid}",
                                 headers=user_headers, timeout=TIMEOUT)
                check(r.status_code == 200, f"Get user 200 (got {r.status_code})")
                if r.ok:
                    p = r.json()
                    for f in ["id", "userName", "email"]:
                        check(f in p, f"Field '{f}' in user profile")
                break

print("\n=== TC023: Update profile ===")
if user_headers:
    r_list = requests.get(f"{BASE_URL}/api/Usuario?pageNumber=1&pageSize=10",
                          headers=admin_headers, timeout=TIMEOUT)
    if r_list.ok and r_list.json().get("results"):
        for u in r_list.json()["results"]:
            if u.get("email") == unique_email:
                uid = u.get("id")
                r = requests.patch(f"{BASE_URL}/api/Usuario/{uid}",
                                   json={"nombreCompleto": "Updated Name", "telefono": "999-8888"},
                                   headers=user_headers, timeout=TIMEOUT)
                check(r.status_code in [200, 204], f"Update profile {r.status_code}")
                break

# --- Admin: list users ---
# --- Books by category ---
print("\n=== TC024: Books by category ===")
r_cats = requests.get(f"{BASE_URL}/api/Categorias", timeout=TIMEOUT)
if r_cats.ok:
    cats = r_cats.json().get("results", [])
    if cats:
        cat_id = cats[0].get("id")
        r = requests.get(f"{BASE_URL}/api/Libro/{cat_id}/libros", timeout=TIMEOUT)
        check(r.status_code == 200, f"Books by category 200 (got {r.status_code})")
        if r.ok:
            p = r.json()
            check("page" in p and "data" in p, "Has page/data fields")

print("\n=== Admin: List users ===")
if admin_headers:
    r = requests.get(f"{BASE_URL}/api/Usuario", headers=admin_headers, timeout=TIMEOUT)
    check(r.status_code == 200, f"Admin list 200 (got {r.status_code})")
    if r.ok:
        d = r.json()
        check("results" in d, "Has 'results' field")
        check(isinstance(d.get("results"), list), "results is list")

# --- Summary ---
print(f"\n{'='*50}")
print(f"Total: {passed} passed, {failed} failed out of {passed+failed}")
if (passed+failed) > 0:
    print(f"Pass rate: {100*passed/(passed+failed):.0f}%")
if failed > 0:
    print("\nFailing results:")
    for status, msg in results:
        if status == "FAIL":
            print(f"  FAIL - {msg}")
