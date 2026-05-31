import requests
import random
import string

BASE_URL = "http://localhost:5179"


def test_category_create_and_get():
    # Generate a random suffix for category name
    rand_suffix = ''.join(random.choices(string.ascii_letters + string.digits, k=8))
    category_name = f"TestCategory_{rand_suffix}"
    category_desc = "Test"

    headers = {
        "Content-Type": "application/json"
    }

    # POST /api/Categorias to create a new category
    post_url = f"{BASE_URL}/api/Categorias"
    post_payload = {
        "nombre": category_name,
        "descripcion": category_desc
    }

    post_response = requests.post(post_url, json=post_payload, headers=headers, timeout=30)
    try:
        assert post_response.status_code == 201, f"Expected status code 201 for POST, got {post_response.status_code}"

        # GET /api/Categorias to list categories
        get_url = f"{BASE_URL}/api/Categorias"
        get_response = requests.get(get_url, headers=headers, timeout=30)
        assert get_response.status_code == 200, f"Expected status code 200 for GET, got {get_response.status_code}"
        data = get_response.json()
        assert "results" in data, "'results' key not found in GET response"
        results = data["results"]
        assert isinstance(results, list), "'results' is not a list"
        # Check newly created category name in results list
        found = any(cat.get("nombre") == category_name for cat in results)
        assert found, f"Created category name '{category_name}' not found in categories list"
    finally:
        # Cleanup: Find the ID of the created category and delete it if possible to clean test data
        try:
            # Find category by name from GET results
            category_id = None
            for cat_item in results:
                if cat_item.get("nombre") == category_name:
                    category_id = cat_item.get("id")
                    break
            if category_id is not None:
                # Attempt to delete category (public DELETE permitted per PRD known limitations)
                delete_url = f"{BASE_URL}/api/Categorias/{category_id}"
                requests.delete(delete_url, headers=headers, timeout=30)
        except Exception:
            pass


test_category_create_and_get()