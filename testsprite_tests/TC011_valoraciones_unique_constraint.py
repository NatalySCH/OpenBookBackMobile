import requests
import uuid

BASE_URL = "http://localhost:5179"
TIMEOUT = 30

def test_valoraciones_unique_constraint():
    # Use provided admin token for auth
    token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW5AZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoiYWRtaW5AZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZWlkZW50aWZpZXIiOiIxNDExNTRmZS1kYWM0LTQyYTQtOWEyMS1lMDdhZGRiYjVkNzYiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbmlzdHJhZG9yIiwiZXhwIjoxNzgwMjU5MTQwLCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MDgwIiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzA4MCJ9.tNPFRJadEUZczha2asiQhC14lqgs95dil7Hp5Haz9Cw"
    
    headers = {"Authorization": f"Bearer {token}", "Content-Type": "application/json"}

    # Step 1: GET a book ID from /api/Libro/paged
    paged_url = f"{BASE_URL}/api/Libro/paged?page=1&pageSize=1"
    try:
        resp = requests.get(paged_url, headers=headers, timeout=TIMEOUT)
        resp.raise_for_status()
    except Exception as e:
        assert False, f"Failed to GET /api/Libro/paged: {e}"

    data = resp.json()
    assert "data" in data and isinstance(data["data"], list) and len(data["data"]) > 0, "No books found in /api/Libro/paged response"
    libro_id = data["data"][0]["id"]
    assert isinstance(libro_id, int), "libro_id is not int"

    # Step 2: POST /api/Valoraciones with {"LibroId":<id>,"Puntuacion":3}
    valoraciones_url = f"{BASE_URL}/api/Valoraciones"
    rating1_payload = {"LibroId": libro_id, "Puntuacion": 3}
    resp1 = requests.post(valoraciones_url, headers=headers, json=rating1_payload, timeout=TIMEOUT)

    assert resp1.status_code == 200, f"First POST /api/Valoraciones failed with status {resp1.status_code}"

    # Step 3: POST again with same LibroId and different Puntuacion (5)
    rating2_payload = {"LibroId": libro_id, "Puntuacion": 5}
    resp2 = requests.post(valoraciones_url, headers=headers, json=rating2_payload, timeout=TIMEOUT)

    assert resp2.status_code == 400, f"Second POST /api/Valoraciones expected 400 but got {resp2.status_code}"

    try:
        error_response = resp2.json()
    except Exception:
        error_response = {}

    mensaje = error_response.get("message", "") or error_response.get("errors") or error_response.get("mensaje") or ""
    assert "Ya has valorado este libro." in str(mensaje), f"Expected error message 'Ya has valorado este libro.' not found in response: {mensaje}"

test_valoraciones_unique_constraint()
