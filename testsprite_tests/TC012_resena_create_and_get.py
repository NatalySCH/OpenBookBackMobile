import requests

BASE_URL = "http://localhost:5179"
AUTH_TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW5AZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoiYWRtaW5AZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZWlkZW50aWZpZXIiOiIxNDExNTRmZS1kYWM0LTQyYTQtOWEyMS1lMDdhZGRiYjVkNzYiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbmlzdHJhZG9yIiwiZXhwIjoxNzgwMjU5MTQwLCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MDgwIiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzA4MCJ9.tNPFRJadEUZczha2asiQhC14lqgs95dil7Hp5Haz9Cw"
HEADERS = {"Authorization": f"Bearer {AUTH_TOKEN}", "Content-Type": "application/json"}
TIMEOUT = 30


def test_resena_create_and_get():
    import json

    # Step 1: Get a book ID from /api/Libro/paged
    paged_url = f"{BASE_URL}/api/Libro/paged?page=1&pageSize=1"
    try:
        paged_resp = requests.get(paged_url, timeout=TIMEOUT)
        assert paged_resp.status_code == 200, f"Failed to get libros paged: {paged_resp.status_code}"
        paged_data = paged_resp.json()
        assert "data" in paged_data and isinstance(paged_data["data"], list) and len(paged_data["data"]) > 0, "No books found in paged data"
        libro_id = paged_data["data"][0]["id"]
        assert isinstance(libro_id, int), "libro_id is not int"
    except Exception as e:
        assert False, f"Error retrieving book id: {str(e)}"

    # Step 2: POST /api/Resena with body {"libroId":<id>,"texto":"Great book!"}
    resena_url = f"{BASE_URL}/api/Resena"
    payload = {"libroId": libro_id, "texto": "Great book!"}
    try:
        post_resp = requests.post(resena_url, headers=HEADERS, json=payload, timeout=TIMEOUT)
    except Exception as e:
        assert False, f"Error posting resena: {str(e)}"
    assert post_resp.status_code in [200, 201], f"Unexpected status code for resena post: {post_resp.status_code}"

test_resena_create_and_get()