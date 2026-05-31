import requests

BASE_URL = "http://localhost:5179"
TIMEOUT = 30

def test_categories_list_paged():
    url = f"{BASE_URL}/api/Categorias"
    try:
        response = requests.get(url, timeout=TIMEOUT)
    except requests.RequestException as e:
        assert False, f"Request to {url} failed: {e}"

    assert response.status_code == 200, f"Expected status code 200, got {response.status_code}"
    try:
        data = response.json()
    except ValueError:
        assert False, "Response is not valid JSON"

    # Assert top-level keys
    expected_keys = {'results', 'totalRecords', 'pageSize', 'currentPage'}
    missing_keys = expected_keys - data.keys()
    assert not missing_keys, f"Response JSON is missing keys: {missing_keys}"

    results = data.get('results')
    assert isinstance(results, list), "'results' is not a list"
    assert len(results) > 0, "'results' list is empty"

    # Validate each item in results
    for item in results:
        assert isinstance(item, dict), "Each item in 'results' must be a dict"
        # Required fields
        for field in ['id', 'nombre', 'totalLibros']:
            assert field in item, f"Item missing required field '{field}'"
        assert isinstance(item['id'], int), "'id' must be an int"
        assert isinstance(item['nombre'], str), "'nombre' must be a string"
        assert isinstance(item['totalLibros'], int), "'totalLibros' must be an int"
        # 'descripcion' must NOT be present
        assert 'descripcion' not in item, "'descripcion' should not be present in category list items"

test_categories_list_paged()