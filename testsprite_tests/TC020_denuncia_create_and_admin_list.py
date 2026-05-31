import requests
import uuid

BASE_URL = "http://localhost:5179"
TIMEOUT = 30

# Predefined admin token from instructions
ADMIN_TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW5AZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoiYWRtaW5AZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZWlkZW50aWZpZXIiOiIxNDExNTRmZS1kYWM0LTQyYTQtOWEyMS1lMDdhZGRiYjVkNzYiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbmlzdHJhZG9yIiwiZXhwIjoxNzgwMjU5MTQwLCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MDgwIiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzA4MCJ9.tNPFRJadEUZczha2asiQhC14lqgs95dil7Hp5Haz9Cw"


def register_user(email: str, password: str) -> dict:
    url = f"{BASE_URL}/api/Auth/register"
    payload = {
        "Correo": email,
        "Contrasena": password
    }
    response = requests.post(url, json=payload, timeout=TIMEOUT)
    # Adjusted to assert status code and handle no JSON on success
    assert response.status_code == 200, f"Registration failed with status {response.status_code} and content {response.text}"
    if response.content:
        try:
            return response.json()
        except Exception:
            return {}
    return {}


def login_user(email: str, password: str) -> str:
    url = f"{BASE_URL}/api/Auth/login"
    payload = {
        "Correo": email,
        "Contrasena": password
    }
    response = requests.post(url, json=payload, timeout=TIMEOUT)
    response.raise_for_status()
    data = response.json()
    token = data.get("token")
    assert token, "Login response missing token"
    return token


def test_denuncia_create_and_admin_list():
    # Generate unique emails for reporter and target users
    unique_suffix_reporter = uuid.uuid4().hex[:8]
    unique_suffix_target = uuid.uuid4().hex[:8]
    reporter_email = f"reporter_{unique_suffix_reporter}@test.com"
    reporter_password = "Test123*"
    target_email = f"target_{unique_suffix_target}@test.com"
    target_password = "Test123*"

    reporter_token = None
    target_id = None

    created_denuncia_id = None

    try:
        # Register reporter user
        register_user(email=reporter_email, password=reporter_password)
        reporter_token = login_user(email=reporter_email, password=reporter_password)

        # Register target user
        register_user(email=target_email, password=target_password)
        # Login target user to ensure created
        login_user(email=target_email, password=target_password)

        # Get target user ID from admin listing users:
        url_users = f"{BASE_URL}/api/Usuario?pageNumber=1&pageSize=100"
        headers_admin = {"Authorization": f"Bearer {ADMIN_TOKEN}"}
        r_users = requests.get(url_users, headers=headers_admin, timeout=TIMEOUT)
        r_users.raise_for_status()
        users_data = r_users.json()
        users_list = users_data.get("results", [])
        # Find target user id by matching email
        for user in users_list:
            if user.get("correo") == target_email or user.get("email") == target_email:
                target_id = user.get("id") or user.get("Id") or user.get("ID")
                break
        # If not found target user id, use admin user id as target per instructions
        if not target_id:
            for user in users_list:
                if user.get("correo") == "admin@gmail.com" or user.get("email") == "admin@gmail.com":
                    target_id = user.get("id") or user.get("Id") or user.get("ID")
                    break

        assert target_id is not None, "Target user id not found"

        # Reporter posts a Denuncia against target user
        denuncia_url = f"{BASE_URL}/api/Denuncia"
        headers_reporter = {"Authorization": f"Bearer {reporter_token}"}
        denuncia_payload = {
            "idDenunciado": int(target_id),  # API expects int
            "comentario": "Test report"
        }
        r_denuncia_post = requests.post(denuncia_url, json=denuncia_payload, headers=headers_reporter, timeout=TIMEOUT)
        assert r_denuncia_post.status_code in [200, 201], f"Denuncia POST failed with status {r_denuncia_post.status_code}"
        # Save created denuncia ID if provided
        try:
            created_denuncia = r_denuncia_post.json()
            created_denuncia_id = created_denuncia.get("id") or created_denuncia.get("Id") or created_denuncia.get("ID")
        except Exception:
            created_denuncia_id = None

        # Admin lists all denuncias
        headers_admin = {"Authorization": f"Bearer {ADMIN_TOKEN}"}
        r_denuncia_get = requests.get(denuncia_url, headers=headers_admin, timeout=TIMEOUT)
        assert r_denuncia_get.status_code == 200, f"Denuncia GET failed with status {r_denuncia_get.status_code}"
        data_get = r_denuncia_get.json()

        # The API may return paged results with 'results', 'data', or the JSON root may be an array
        if isinstance(data_get, list):
            results_array = data_get
        else:
            results_array = data_get.get("results") or data_get.get("data")

        assert results_array is not None and isinstance(results_array, list), "Denuncia GET response missing results array"

    finally:
        # Cleanup - delete the created Denuncia and created users if possible
        if created_denuncia_id:
            try:
                delete_url = f"{BASE_URL}/api/Denuncia/{created_denuncia_id}"
                headers_admin = {"Authorization": f"Bearer {ADMIN_TOKEN}"}
                r_del = requests.delete(delete_url, headers=headers_admin, timeout=TIMEOUT)
                assert r_del.status_code in [200, 204, 404]
            except Exception:
                pass

        # Delete reporter user if exists
        try:
            headers_admin = {"Authorization": f"Bearer {ADMIN_TOKEN}"}
            r_all_users = requests.get(f"{BASE_URL}/api/Usuario?pageNumber=1&pageSize=100", headers=headers_admin, timeout=TIMEOUT)
            r_all_users.raise_for_status()
            users_list = r_all_users.json().get("results", [])
            reporter_id_del = None
            for user in users_list:
                if user.get("correo") == reporter_email or user.get("email") == reporter_email:
                    reporter_id_del = user.get("id") or user.get("Id") or user.get("ID")
                    break
            if reporter_id_del:
                del_url = f"{BASE_URL}/api/Usuario/{reporter_id_del}"
                requests.delete(del_url, headers=headers_admin, timeout=TIMEOUT)
        except Exception:
            pass

        # Delete target user if created and is not admin
        try:
            if target_id and target_email != "admin@gmail.com":
                headers_admin = {"Authorization": f"Bearer {ADMIN_TOKEN}"}
                del_target_url = f"{BASE_URL}/api/Usuario/{target_id}"
                requests.delete(del_target_url, headers=headers_admin, timeout=TIMEOUT)
        except Exception:
            pass


test_denuncia_create_and_admin_list()
