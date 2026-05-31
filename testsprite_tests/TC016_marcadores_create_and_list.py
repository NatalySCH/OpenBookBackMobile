import requests

BASE_URL = "http://localhost:5179"
TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW5AZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoiYWRtaW5AZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZWlkZW50aWZpZXIiOiIxNDExNTRmZS1kYWM0LTQyYTQtOWEyMS1lMDdhZGRiYjVkNzYiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbmlzdHJhZG9yIiwiZXhwIjoxNzgwMjU5MTQwLCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MDgwIiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzA4MCJ9.tNPFRJadEUZczha2asiQhC14lqgs95dil7Hp5Haz9Cw"
HEADERS = {
    "Authorization": f"Bearer {TOKEN}",
    "Content-Type": "application/json"
}
TIMEOUT = 30


def test_marcadores_create_and_list():
    # POST /api/Marcadores with {"libroId":1,"pagina":10}
    post_url = f"{BASE_URL}/api/Marcadores"
    post_payload = {"libroId": 1, "pagina": 10}

    try:
        post_response = requests.post(post_url, json=post_payload, headers=HEADERS, timeout=TIMEOUT)
    except requests.RequestException as e:
        assert False, f"POST /api/Marcadores request failed: {str(e)}"
    assert post_response.status_code in [200, 201, 404], f"Unexpected POST status code: {post_response.status_code}"

    # GET /api/Marcadores/usuario
    get_url = f"{BASE_URL}/api/Marcadores/usuario"
    try:
        get_response = requests.get(get_url, headers=HEADERS, timeout=TIMEOUT)
    except requests.RequestException as e:
        assert False, f"GET /api/Marcadores/usuario request failed: {str(e)}"
    assert get_response.status_code == 200, f"Unexpected GET status code: {get_response.status_code}"
    # Check that response is a JSON list (could be empty)
    try:
        data = get_response.json()
    except ValueError:
        assert False, "GET /api/Marcadores/usuario did not return JSON"
    assert isinstance(data, list), f"Expected list of bookmarks, got: {type(data)}"


test_marcadores_create_and_list()