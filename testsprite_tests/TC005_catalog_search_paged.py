import requests

BASE_URL = "http://localhost:5179"
TIMEOUT = 30

def test_catalog_search_paged():
    url = f"{BASE_URL}/api/Libro/paged?page=1&pageSize=10"
    headers = {}
    try:
        response = requests.get(url, headers=headers, timeout=TIMEOUT)
        assert response.status_code == 200, f"Expected status 200, got {response.status_code}"
        data = response.json()
        # Validate presence and types of pagination fields
        assert 'page' in data and isinstance(data['page'], int), "'page' missing or not int"
        assert 'pageSize' in data and isinstance(data['pageSize'], int), "'pageSize' missing or not int"
        assert 'total' in data and isinstance(data['total'], int), "'total' missing or not int"
        assert 'totalPages' in data and isinstance(data['totalPages'], int), "'totalPages' missing or not int"
        assert 'data' in data and isinstance(data['data'], list), "'data' missing or not a list"

        expected_fields = {
            'id': int,
            'titulo': str,
            'autor': str,
            'descripcion': str,
            'portadaUrl': (str, type(None)),
            'archivoUrl': str,
            'esPublico': bool,
            'fechaCreacion': str,
            'usuarioCreadorId': str,
            'promedioValoracion': (int, float),
            'totalValoraciones': int,
            'categorias': list
        }

        for item in data['data']:
            for field, field_type in expected_fields.items():
                assert field in item, f"Field '{field}' missing in data item"
                # Check type, including None for portadaUrl
                if isinstance(field_type, tuple):
                    assert any(isinstance(item[field], t) for t in field_type), \
                        f"Field '{field}' is not of expected types {field_type}"
                else:
                    assert isinstance(item[field], field_type), f"Field '{field}' is not of type {field_type}"
            # categorias must be list of strings
            assert all(isinstance(cat, str) for cat in item['categorias']), "'categorias' must be list of strings"
            # descripcion must be present and NOT 'sinopsis'
            assert 'sinopsis' not in item, "'sinopsis' field should not be present"

    except requests.RequestException as e:
        assert False, f"Request failed: {e}"

test_catalog_search_paged()