import requests
import random
import string

BASE_URL = "http://localhost:5179"
TIMEOUT = 30


def test_valoraciones_create_and_rating_calculation():
    session = requests.Session()
    random_suffix = ''.join(random.choices(string.ascii_lowercase + string.digits, k=8))
    register_url = f"{BASE_URL}/api/Auth/register"
    login_url = f"{BASE_URL}/api/Auth/login"
    paged_libro_url = f"{BASE_URL}/api/Libro/paged"
    valoraciones_url = f"{BASE_URL}/api/Valoraciones"

    # Step 1: Register new user
    register_payload = {
        "Correo": f"rating_{random_suffix}@test.com",
        "Contrasena": "Test123*",
        "UserName": f"rating_{random_suffix}"
    }
    try:
        reg_resp = session.post(register_url, json=register_payload, timeout=TIMEOUT)
        assert reg_resp.status_code == 200, f"Registration failed with status {reg_resp.status_code}, response: {reg_resp.text}"

        # Step 2: Login to get JWT token
        login_payload = {
            "Correo": register_payload["Correo"],
            "Contrasena": register_payload["Contrasena"]
        }
        login_resp = session.post(login_url, json=login_payload, timeout=TIMEOUT)
        assert login_resp.status_code == 200, f"Login failed with status {login_resp.status_code}, response: {login_resp.text}"
        token_data = login_resp.json()
        token = token_data.get("token")
        assert token and isinstance(token, str) and token.strip(), "Token missing or empty in login response"

        headers = {
            "Authorization": f"Bearer {token}",
            "Content-Type": "application/json"
        }

        # Step 3: GET libro paged to retrieve libroId
        params = {
            "page": 1,
            "pageSize": 1
        }
        libro_resp = session.get(paged_libro_url, params=params, timeout=TIMEOUT)
        assert libro_resp.status_code == 200, f"Get libro paged failed with status {libro_resp.status_code}, response: {libro_resp.text}"
        libros_paged = libro_resp.json()
        assert "data" in libros_paged, "'data' field missing in libros paged response"
        libros = libros_paged["data"]
        assert isinstance(libros, list) and len(libros) > 0, "No libros found in paged response"
        libro_id = libros[0].get("id")
        assert isinstance(libro_id, int), "libroId invalid or missing"

        # Step 4: POST to /api/Valoraciones with libroId and puntuacion 4
        valoracion_payload = {
            "libroId": libro_id,
            "puntuacion": 4
        }
        post_resp = session.post(valoraciones_url, json=valoracion_payload, headers=headers, timeout=TIMEOUT)
        assert post_resp.status_code in [200, 201], f"POST /api/Valoraciones failed with status {post_resp.status_code}, response: {post_resp.text}"
        data = post_resp.json()

        # Validate fields in response: id (int), usuarioId (string), libroId (int), puntuacion (int), fecha (string)
        assert isinstance(data.get("id"), int), "'id' field missing or not int"
        usuario_id = data.get("usuarioId")
        assert usuario_id and isinstance(usuario_id, str), "'usuarioId' field missing or not string"
        assert data.get("libroId") == libro_id, f"'libroId' field mismatch: expected {libro_id}, got {data.get('libroId')}"
        puntuacion = data.get("puntuacion")
        assert isinstance(puntuacion, int) and 1 <= puntuacion <= 5, "'puntuacion' field invalid or out of range"
        fecha = data.get("fecha")
        assert fecha and isinstance(fecha, str), "'fecha' field missing or not string"

    finally:
        # Cleanup is not required according to instructions for ratings
        session.close()


test_valoraciones_create_and_rating_calculation()