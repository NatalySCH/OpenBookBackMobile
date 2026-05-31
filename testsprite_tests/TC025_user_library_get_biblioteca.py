import requests
import uuid

BASE_URL = "http://localhost:5179"
TIMEOUT = 30

def test_user_library_get_biblioteca():
    # Use provided admin token from metadata
    token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW5AZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoiYWRtaW5AZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZWlkZW50aWZpZXIiOiIxNDExNTRmZS1kYWM0LTQyYTQtOWEyMS1lMDdhZGRiYjVkNzYiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbmlzdHJhZG9yIiwiZXhwIjoxNzgwMjU5MTQwLCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MDgwIiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzA4MCJ9.tNPFRJadEUZczha2asiQhC14lqgs95dil7Hp5Haz9Cw"
    
    headers = {
        "Authorization": f"Bearer {token}",
        "Content-Type": "application/json"
    }

    # Step 1: GET a book ID from /api/Libro/paged to have a libroId to use for adding to library
    try:
        resp = requests.get(f"{BASE_URL}/api/Libro/paged?page=1&pageSize=1", timeout=TIMEOUT)
        resp.raise_for_status()
    except Exception as e:
        assert False, f"Failed to get a book for test setup: {str(e)}"
    
    books_data = resp.json()
    assert "data" in books_data and isinstance(books_data["data"], list) and len(books_data["data"]) > 0, "No book data found to add to library"
    libroId = books_data["data"][0]["id"]
    
    # Step 2: Add the book to the user library POST /api/UsuarioLibro/{libroId}
    add_url = f"{BASE_URL}/api/UsuarioLibro/{libroId}"
    try:
        add_resp = requests.post(add_url, headers=headers, timeout=TIMEOUT)
    except Exception as e:
        assert False, f"Failed to add book to library: {str(e)}"
    assert add_resp.status_code in [200, 201], f"Expected 200/201 on adding to library, got {add_resp.status_code}"
    
    # Step 3: GET the user library /api/UsuarioLibro/biblioteca
    try:
        get_resp = requests.get(f"{BASE_URL}/api/UsuarioLibro/biblioteca", headers=headers, timeout=TIMEOUT)
    except Exception as e:
        assert False, f"Failed to get user library: {str(e)}"
    assert get_resp.status_code == 200, f"Expected 200 OK on getting library, got {get_resp.status_code}"

    data = get_resp.json()
    assert isinstance(data, list), "Response is not a list"
    # Check each item has the required fields
    required_fields = {"LibroId", "Titulo", "Autor", "Progreso", "EsFavorito", "PaginaActual", "UltimaLectura"}
    for item in data:
        assert isinstance(item, dict), "Library item is not an object"
        missing = required_fields - item.keys()
        assert not missing, f"Library item missing fields: {missing}"

    # Clean up: remove the book from the user library
    try:
        del_resp = requests.delete(add_url, headers=headers, timeout=TIMEOUT)
        # We expect success codes 200 or 204 for deletion
        assert del_resp.status_code in [200, 204], f"Failed to delete library book, status {del_resp.status_code}"
    except Exception as e:
        # Log error but don't fail since cleanup
        print(f"Warning: failed to cleanup library book: {e}")

test_user_library_get_biblioteca()
