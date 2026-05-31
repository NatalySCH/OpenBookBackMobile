import requests

BASE_URL = "http://localhost:5179"
TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW5AZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoiYWRtaW5AZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZWlkZW50aWZpZXIiOiIxNDExNTRmZS1kYWM0LTQyYTQtOWEyMS1lMDdhZGRiYjVkNzYiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbmlzdHJhZG9yIiwiZXhwIjoxNzgwMjU5MTQwLCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MDgwIiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzA4MCJ9.tNPFRJadEUZczha2asiQhC14lqgs95dil7Hp5Haz9Cw"
HEADERS = {"Authorization": f"Bearer {TOKEN}"}
TIMEOUT = 30

def test_usuario_libro_update_progreso():
    libro_id = None
    try:
        # Step 1: Get list of books to find a libroId
        resp_catalog = requests.get(f"{BASE_URL}/api/Libro/paged?page=1&pageSize=1", timeout=TIMEOUT)
        assert resp_catalog.status_code == 200, f"Expected 200 from Libro paged, got {resp_catalog.status_code}"
        data_catalog = resp_catalog.json()
        assert "data" in data_catalog and isinstance(data_catalog["data"], list) and len(data_catalog["data"]) > 0, "No books in catalog data"
        libro_id = data_catalog["data"][0]["id"]

        # Step 2: Add book to library via POST /api/UsuarioLibro/{libroId}
        resp_add = requests.post(f"{BASE_URL}/api/UsuarioLibro/{libro_id}", headers=HEADERS, timeout=TIMEOUT)
        # According to TC013, Accept 200 or 201, but here we expect 200 to proceed
        assert resp_add.status_code in [200, 201], f"Expected 200 or 201 adding book to library, got {resp_add.status_code}"

        # Step 3: PATCH /api/UsuarioLibro/progreso with JSON body - keys case fixed
        patch_url = f"{BASE_URL}/api/UsuarioLibro/progreso"
        patch_body = {
            "LibroId": libro_id,
            "Progreso": 0.5,
            "PaginaActual": 15
        }
        resp_patch = requests.patch(patch_url, headers={**HEADERS, "Content-Type": "application/json"}, json=patch_body, timeout=TIMEOUT)

        assert resp_patch.status_code in [200, 204], f"Expected status code 200 or 204 from progreso patch, got {resp_patch.status_code}"

    finally:
        # Cleanup: Remove the book from user's library if libro_id was set
        if libro_id is not None:
            try:
                resp_del = requests.delete(f"{BASE_URL}/api/UsuarioLibro/{libro_id}", headers=HEADERS, timeout=TIMEOUT)
                # No strict assertion here in finally to avoid masks, but log if failed
                if resp_del.status_code not in [200, 204]:
                    print(f"Cleanup failed: DELETE /api/UsuarioLibro/{libro_id} returned {resp_del.status_code}")
            except Exception as e:
                print(f"Cleanup exception: {e}")

test_usuario_libro_update_progreso()
