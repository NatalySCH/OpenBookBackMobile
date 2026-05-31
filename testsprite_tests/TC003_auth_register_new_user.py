import requests
import random
import string

BASE_URL = "http://localhost:5179"

def test_auth_register_new_user():
    # Generate random email and username
    rand_str = ''.join(random.choices(string.ascii_lowercase + string.digits, k=8))
    email = f"test_new_{rand_str}@test.com"
    username = f"testuser_{rand_str}"

    url = f"{BASE_URL}/api/Auth/register"
    headers = {
        "Content-Type": "application/json"
    }
    payload = {
        "Correo": email,
        "Contrasena": "Test123*",
        "UserName": username
    }

    try:
        response = requests.post(url, json=payload, headers=headers, timeout=30)
    except requests.RequestException as e:
        assert False, f"Request failed: {e}"

    assert response.status_code == 200, f"Expected status code 200 but got {response.status_code}"

test_auth_register_new_user()