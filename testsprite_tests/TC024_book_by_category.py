import requests

BASE_URL = "http://localhost:5179"
TIMEOUT = 30

def test_book_by_category():
    # Step 1: Get categories
    url_categories = f"{BASE_URL}/api/Categorias"
    try:
        response_categories = requests.get(url_categories, timeout=TIMEOUT)
        assert response_categories.status_code == 200, f"Expected 200, got {response_categories.status_code}"
        categories_json = response_categories.json()
        # Accept different possible paged result keys for categories list
        if "results" in categories_json:
            categories_list = categories_json["results"]
        elif "data" in categories_json:
            categories_list = categories_json["data"]
        else:
            categories_list = categories_json

        assert isinstance(categories_list, list), "Categories response does not contain a list"
        assert len(categories_list) > 0, "No categories found"

        # Use the first category's id
        first_category = categories_list[0]
        categoria_id = None
        # Try common id keys
        if isinstance(first_category, dict):
            if "id" in first_category:
                categoria_id = first_category["id"]
            elif "Id" in first_category:
                categoria_id = first_category["Id"]
        assert categoria_id is not None, "Category ID not found in response"
    except Exception as e:
        raise AssertionError(f"Failed getting categories: {str(e)}")

    # Step 2: Get books by category
    url_books = f"{BASE_URL}/api/Libro/{categoria_id}/libros"
    try:
        response_books = requests.get(url_books, timeout=TIMEOUT)
        assert response_books.status_code == 200, f"Expected 200, got {response_books.status_code}"
        books_json = response_books.json()
        # Accept list or paged result containing the list
        if isinstance(books_json, dict):
            if "results" in books_json:
                books_list = books_json["results"]
            elif "data" in books_json:
                books_list = books_json["data"]
            else:
                # If not recognized, raise error
                raise AssertionError("Books response dict does not contain list in 'results' or 'data'")
        elif isinstance(books_json, list):
            books_list = books_json
        else:
            raise AssertionError("Books response is neither list nor dict with expected keys")
        assert isinstance(books_list, list), "Books list is not a list"
    except Exception as e:
        raise AssertionError(f"Failed getting books by category: {str(e)}")

test_book_by_category()