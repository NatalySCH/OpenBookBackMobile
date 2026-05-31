import requests

BASE_URL = "http://localhost:5179"
TIMEOUT = 30

def test_usuario_get_profile():
    admin_login_url = f"{BASE_URL}/api/Auth/login"
    admin_login_payload = {
        "Correo": "admin@gmail.com",
        "Contrasena": "Admin123*"
    }
    # Login as admin to get admin token
    try:
        admin_login_resp = requests.post(admin_login_url, json=admin_login_payload, timeout=TIMEOUT)
        assert admin_login_resp.status_code == 200, f"Admin login failed with status {admin_login_resp.status_code}"
        admin_data = admin_login_resp.json()
        admin_token = admin_data.get("token")
        assert admin_token, "Admin token missing in login response"
    except Exception as e:
        raise AssertionError(f"Admin login request failed: {e}")

    # Use admin token to get user list with pagination
    user_list_url = f"{BASE_URL}/api/Usuario"
    headers_admin = {
        "Authorization": f"Bearer {admin_token}"
    }
    params_user_list = {
        "pageNumber": 1,
        "pageSize": 10
    }
    try:
        user_list_resp = requests.get(user_list_url, headers=headers_admin, params=params_user_list, timeout=TIMEOUT)
        assert user_list_resp.status_code == 200, f"Get user list failed with status {user_list_resp.status_code}"
        user_list_data = user_list_resp.json()
        assert "results" in user_list_data and isinstance(user_list_data["results"], list)
        assert len(user_list_data["results"]) > 0, "User list is empty"
        first_user = user_list_data["results"][0]
        user_id = first_user.get("id")
        assert user_id, "User ID missing in user list first record"
    except Exception as e:
        raise AssertionError(f"Getting user list failed: {e}")

    user_email = first_user.get("email") or first_user.get("correo")
    if not user_email:
        raise AssertionError("User email missing in user data")

    # Try login as user with password "Admin123*" (hope password equals)
    user_login_url = f"{BASE_URL}/api/Auth/login"
    user_login_payload = {
        "Correo": user_email,
        "Contrasena": "Admin123*"
    }
    try:
        user_login_resp = requests.post(user_login_url, json=user_login_payload, timeout=TIMEOUT)
        if user_login_resp.status_code != 200:
            raise AssertionError(f"User login failed with status {user_login_resp.status_code}")
        user_login_data = user_login_resp.json()
        user_token = user_login_data.get("token")
        assert user_token, "User token missing in login response"
    except Exception as e:
        raise AssertionError(f"User login request failed: {e}")

    # Use user token to GET /api/Usuario/{id}
    user_profile_url = f"{BASE_URL}/api/Usuario/{user_id}"
    headers_user = {
        "Authorization": f"Bearer {user_token}"
    }
    try:
        user_profile_resp = requests.get(user_profile_url, headers=headers_user, timeout=TIMEOUT)
        assert user_profile_resp.status_code == 200, f"Get user profile failed with status {user_profile_resp.status_code}"
        user_profile_data = user_profile_resp.json()
        # Assert required fields without 'fechaNacimiento'
        required_fields = [
            "id", "userName", "email", "nombreCompleto",
            "fotoPerfilUrl", "pais", "telefono"
        ]
        for field in required_fields:
            assert field in user_profile_data, f"Field '{field}' missing in user profile response"
    except Exception as e:
        raise AssertionError(f"Getting user profile failed: {e}")

test_usuario_get_profile()
