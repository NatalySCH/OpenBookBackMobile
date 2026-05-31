import requests

def test_auth_login_correct_credentials():
    base_url = "http://localhost:5179"
    url = f"{base_url}/api/Auth/login"
    payload = {
        "Correo": "admin@gmail.com",
        "Contrasena": "Admin123*"
    }
    headers = {
        "Content-Type": "application/json"
    }
    try:
        response = requests.post(url, json=payload, headers=headers, timeout=30)
        assert response.status_code == 200, f"Expected status code 200, got {response.status_code}"
        data = response.json()
        assert "token" in data, "Response JSON missing 'token'"
        assert isinstance(data["token"], str) and data["token"].strip() != "", "'token' is empty or not a string"
        assert "username" in data, "Response JSON missing 'username'"
        assert isinstance(data["username"], str), "'username' is not a string"
        assert "correo" in data, "Response JSON missing 'correo' (IMPORTANT: field is not 'email')"
        assert isinstance(data["correo"], str), "'correo' is not a string"
        assert "fotoPerfilUrl" in data, "Response JSON missing 'fotoPerfilUrl'"
        assert (isinstance(data["fotoPerfilUrl"], str) or data["fotoPerfilUrl"] is None), "'fotoPerfilUrl' is not a string or null"
    except requests.RequestException as e:
        assert False, f"Request failed: {e}"

test_auth_login_correct_credentials()