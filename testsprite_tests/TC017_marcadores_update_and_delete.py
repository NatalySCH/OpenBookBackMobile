import requests

BASE_URL = "http://localhost:5179"
TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW5AZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoiYWRtaW5AZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZWlkZW50aWZpZXIiOiIxNDExNTRmZS1kYWM0LTQyYTQtOWEyMS1lMDdhZGRiYjVkNzYiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbmlzdHJhZG9yIiwiZXhwIjoxNzgwMjU5MTQwLCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MDgwIiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzA4MCJ9.tNPFRJadEUZczha2asiQhC14lqgs95dil7Hp5Haz9Cw"
HEADERS = {"Authorization": f"Bearer {TOKEN}", "Content-Type": "application/json"}
TIMEOUT = 30

def test_marcadores_update_and_delete():
    bookmark_id = None
    try:
        # Create bookmark
        create_payload = {"libroId": 1, "pagina": 10}
        resp_create = requests.post(f"{BASE_URL}/api/Marcadores", headers=HEADERS, json=create_payload, timeout=TIMEOUT)
        assert resp_create.status_code == 200, f"Create bookmark failed with status {resp_create.status_code}"
        create_data = resp_create.json()
        # Extract created bookmark id
        if isinstance(create_data, dict) and "id" in create_data:
            bookmark_id = create_data["id"]
        else:
            # Sometimes the id might be the return itself or not included, try to get from response headers or fail
            raise AssertionError("Response JSON does not contain 'id' field on bookmark creation")

        # Update bookmark
        update_payload = {"pagina": 20}
        resp_update = requests.put(f"{BASE_URL}/api/Marcadores/{bookmark_id}", headers=HEADERS, json=update_payload, timeout=TIMEOUT)
        assert resp_update.status_code in (200, 204), f"Update bookmark failed with status {resp_update.status_code}"

        # Delete bookmark
        resp_delete = requests.delete(f"{BASE_URL}/api/Marcadores/{bookmark_id}", headers=HEADERS, timeout=TIMEOUT)
        # According to doc, delete returns 200 or 204
        assert resp_delete.status_code in (200, 204), f"Delete bookmark failed with status {resp_delete.status_code}"
        bookmark_id = None  # Deleted successfully
    finally:
        # Cleanup if not deleted
        if bookmark_id is not None:
            try:
                requests.delete(f"{BASE_URL}/api/Marcadores/{bookmark_id}", headers=HEADERS, timeout=TIMEOUT)
            except Exception:
                pass

test_marcadores_update_and_delete()
