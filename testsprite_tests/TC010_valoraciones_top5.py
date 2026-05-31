import requests

def test_valoraciones_top5():
    base_url = "http://localhost:5179"
    url = f"{base_url}/api/Valoraciones/top5"
    timeout = 30

    try:
        response = requests.get(url, timeout=timeout)
        assert response.status_code == 200, f"Expected status code 200 but got {response.status_code}"

        data = response.json()
        assert isinstance(data, list), f"Expected response to be a list but got {type(data)}"

        # Each item must have fields: LibroId, Titulo, Promedio, TotalValoraciones
        for idx, item in enumerate(data):
            assert isinstance(item, dict), f"Item at index {idx} is not a dict"
            expected_fields = {'LibroId', 'Titulo', 'Promedio', 'TotalValoraciones'}
            item_keys = set(item.keys())
            missing_fields = expected_fields - item_keys
            assert not missing_fields, f"Item at index {idx} missing fields: {missing_fields}"
    except requests.RequestException as e:
        assert False, f"Request failed: {e}"

test_valoraciones_top5()