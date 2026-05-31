import requests

base_url = "http://localhost:5179"

def test_usuario_libro_add_to_library():
    import uuid

    session = requests.Session()
    timeout = 30

    # Register a new user
    random_suffix = str(uuid.uuid4())[:8]
    register_url = f"{base_url}/api/Auth/register"
    user_email = f"user_addlib_{random_suffix}@test.com"
    user_password = "Test123*"
    register_body = {
        "Correo": user_email,
        "Contrasena": user_password,
        "UserName": f"user_{random_suffix}"
    }
    try:
        register_resp = session.post(register_url, json=register_body, timeout=timeout)
        assert register_resp.status_code == 200, f"Register failed: {register_resp.status_code} {register_resp.text}"

        # Login the new user to get token
        login_url = f"{base_url}/api/Auth/login"
        login_body = {
            "Correo": user_email,
            "Contrasena": user_password
        }
        login_resp = session.post(login_url, json=login_body, timeout=timeout)
        assert login_resp.status_code == 200, f"Login failed: {login_resp.status_code} {login_resp.text}"
        login_data = login_resp.json()
        assert "token" in login_data and isinstance(login_data["token"], str) and login_data["token"], "Missing or empty token in login response"
        token = login_data["token"]

        headers = {"Authorization": f"Bearer {token}"}

        # Get a book ID from /api/Libro/paged
        paged_url = f"{base_url}/api/Libro/paged?page=1&pageSize=10"
        paged_resp = session.get(paged_url, timeout=timeout)
        assert paged_resp.status_code == 200, f"Paged books request failed: {paged_resp.status_code} {paged_resp.text}"
        paged_data = paged_resp.json()
        # Validate presence of 'data' and that it's a list with at least one book
        assert "data" in paged_data and isinstance(paged_data["data"], list) and len(paged_data["data"]) > 0, "No books found in paged data"
        first_book = paged_data["data"][0]
        assert "id" in first_book and isinstance(first_book["id"], int), "Book object missing 'id'"
        libro_id = first_book["id"]

        # POST /api/UsuarioLibro/{libroId} with Bearer token and empty body
        add_url = f"{base_url}/api/UsuarioLibro/{libro_id}"
        post_resp = session.post(add_url, headers=headers, json={}, timeout=timeout)
        assert post_resp.status_code == 200, f"Add to library failed with status {post_resp.status_code}: {post_resp.text}"

    finally:
        # Cleanup: delete the added book from user library if possible
        # Using the same token, attempt DELETE /api/UsuarioLibro/{libroId}
        try:
            if 'token' in locals() and 'libro_id' in locals():
                del_url = f"{base_url}/api/UsuarioLibro/{libro_id}"
                del_resp = session.delete(del_url, headers={"Authorization": f"Bearer {token}"}, timeout=timeout)
                # no strict assertion here, just attempt cleanup
        except Exception:
            pass

test_usuario_libro_add_to_library()
