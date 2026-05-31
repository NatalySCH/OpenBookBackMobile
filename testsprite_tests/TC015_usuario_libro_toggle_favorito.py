import requests

BASE_URL = "http://localhost:5179"
TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW5AZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoiYWRtaW5AZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZWlkZW50aWZpZXIiOiIxNDExNTRmZS1kYWM0LTQyYTQtOWEyMS1lMDdhZGRiYjVkNzYiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbmlzdHJhZG9yIiwiZXhwIjoxNzgwMjU5MTQwLCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MDgwIiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzA4MCJ9.tNPFRJadEUZczha2asiQhC14lqgs95dil7Hp5Haz9Cw"
HEADERS = {
    "Authorization": f"Bearer {TOKEN}",
    "Content-Type": "application/json",
    "Accept": "application/json"
}
TIMEOUT = 30

def test_usuario_libro_toggle_favorito():
    # Step 1: Get a libroId from public catalog
    try:
        resp_catalog = requests.get(f"{BASE_URL}/api/Libro", timeout=TIMEOUT)
        resp_catalog.raise_for_status()
        libros = resp_catalog.json()
        assert isinstance(libros, list) and len(libros) > 0, "No books available in catalog"
        libro_id = libros[0]["id"]
    except Exception as e:
        raise AssertionError(f"Failed to get libroId from catalog: {e}")

    # Step 2: Add book to user's library via POST /api/UsuarioLibro/{libroId} with auth
    url_add = f"{BASE_URL}/api/UsuarioLibro/{libro_id}"
    try:
        resp_add = requests.post(url_add, headers=HEADERS, timeout=TIMEOUT)
        if resp_add.status_code not in [200, 201, 400]:
            resp_add.raise_for_status()
        # If already in library 400, continue anyway
    except Exception as e:
        raise AssertionError(f"Failed to add book to library: {e}")

    # Step 3: Toggle favorito status via PATCH /api/UsuarioLibro/favorito/{libroId} with empty body and auth
    url_toggle = f"{BASE_URL}/api/UsuarioLibro/favorito/{libro_id}"
    try:
        resp_toggle = requests.patch(url_toggle, headers=HEADERS, json={}, timeout=TIMEOUT)
        assert resp_toggle.status_code in [200, 204], f"Unexpected status code toggling favorito: {resp_toggle.status_code}"
    except Exception as e:
        raise AssertionError(f"Failed to toggle favorito status: {e}")

    # Step 4: Clean up by deleting the book from user's library
    try:
        resp_delete = requests.delete(url_add, headers=HEADERS, timeout=TIMEOUT)
        if resp_delete.status_code not in [200, 204, 404]:
            resp_delete.raise_for_status()
    except Exception as e:
        # Log but do not fail test on cleanup
        pass

test_usuario_libro_toggle_favorito()