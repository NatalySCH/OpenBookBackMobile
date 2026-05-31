import requests

BASE_URL = "http://localhost:5179"
TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW5AZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoiYWRtaW5AZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZWlkZW50aWZpZXIiOiIxNDExNTRmZS1kYWM0LTQyYTQtOWEyMS1lMDdhZGRiYjVkNzYiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbmlzdHJhZG9yIiwiZXhwIjoxNzgwMjU5MTQwLCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MDgwIiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzA4MCJ9.tNPFRJadEUZczha2asiQhC14lqgs95dil7Hp5Haz9Cw"
HEADERS = {"Authorization": f"Bearer {TOKEN}", "Content-Type": "application/json"}
TIMEOUT = 30

def test_resaltadores_update_and_delete():
    # Create highlight
    post_url = f"{BASE_URL}/api/Resaltadores"
    post_payload = {
        "LibroId": 1,
        "Href": "text.html",
        "CfiRange": "/6/4[chap]!/4/2/1:0",
        "Color": "#FFFF00"
    }

    response_post = requests.post(post_url, json=post_payload, headers=HEADERS, timeout=TIMEOUT)
    assert response_post.status_code == 200, f"Expected 200 for POST, got {response_post.status_code}"
    highlight = response_post.json()
    highlight_id = highlight.get("id")
    assert highlight_id is not None, "Response JSON does not contain 'id'"

    try:
        # Update highlight
        put_url = f"{BASE_URL}/api/Resaltadores/{highlight_id}"
        put_payload = {
            "Color": "#FF0000",
            "CfiRange": "/6/4[chap]!/4/2/1:50"
        }

        response_put = requests.put(put_url, json=put_payload, headers=HEADERS, timeout=TIMEOUT)
        assert response_put.status_code in (200, 204), f"Expected 200 or 204 for PUT, got {response_put.status_code}"

        # Delete highlight
        delete_url = put_url
        response_delete = requests.delete(delete_url, headers=HEADERS, timeout=TIMEOUT)
        assert response_delete.status_code in (200, 204), f"Expected 200 or 204 for DELETE, got {response_delete.status_code}"

        # Try deleting again to check 404
        response_delete_again = requests.delete(delete_url, headers=HEADERS, timeout=TIMEOUT)
        assert response_delete_again.status_code == 404, f"Expected 404 for DELETE again, got {response_delete_again.status_code}"

    finally:
        # Cleanup if still exists
        verify_response = requests.get(f"{BASE_URL}/api/Resaltadores/{highlight_id}", headers=HEADERS, timeout=TIMEOUT)
        if verify_response.status_code == 200:
            requests.delete(delete_url, headers=HEADERS, timeout=TIMEOUT)

test_resaltadores_update_and_delete()
