import requests

def test_auth_login_incorrect_password():
    base_url = "http://localhost:5179"
    url = f"{base_url}/api/Auth/login"
    payload = {
        "Correo": "admin@gmail.com",
        "Contrasena": "wrong"
    }
    headers = {
        "Content-Type": "application/json"
    }
    try:
        response = requests.post(url, json=payload, headers=headers, timeout=30)
    except requests.RequestException as e:
        assert False, f"Request failed: {e}"
    assert response.status_code == 401, f"Expected status code 401 but got {response.status_code}"

test_auth_login_incorrect_password()