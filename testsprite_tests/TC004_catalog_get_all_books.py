import requests

def test_catalog_get_all_books():
    base_url = "http://localhost:5179"
    url = f"{base_url}/api/Libro"
    timeout = 30

    try:
        response = requests.get(url, timeout=timeout)
    except requests.RequestException as e:
        assert False, f"Request failed: {e}"

    assert response.status_code == 200, f"Expected status code 200, got {response.status_code}"

    try:
        books = response.json()
    except ValueError:
        assert False, "Response is not valid JSON"

    assert isinstance(books, list), "Response JSON is not a list"
    assert len(books) > 0, "Response JSON list is empty"

    for book in books:
        assert isinstance(book, dict), "Book item is not a dictionary"
        # Validate fields
        # id (int)
        assert "id" in book and isinstance(book["id"], int), "Missing or invalid 'id'"
        # titulo (string)
        assert "titulo" in book and isinstance(book["titulo"], str), "Missing or invalid 'titulo'"
        # autor (string)
        assert "autor" in book and isinstance(book["autor"], str), "Missing or invalid 'autor'"
        # descripcion (string)
        assert "descripcion" in book and isinstance(book["descripcion"], str), "Missing or invalid 'descripcion'"
        # portadaUrl (string or null)
        assert "portadaUrl" in book and (isinstance(book["portadaUrl"], str) or book["portadaUrl"] is None), "Missing or invalid 'portadaUrl'"
        # archivoUrl (string)
        assert "archivoUrl" in book and isinstance(book["archivoUrl"], str), "Missing or invalid 'archivoUrl'"
        # esPublico (bool)
        assert "esPublico" in book and isinstance(book["esPublico"], bool), "Missing or invalid 'esPublico'"
        # fechaCreacion (string)
        assert "fechaCreacion" in book and isinstance(book["fechaCreacion"], str), "Missing or invalid 'fechaCreacion'"
        # usuarioCreadorId (string)
        assert "usuarioCreadorId" in book and isinstance(book["usuarioCreadorId"], str), "Missing or invalid 'usuarioCreadorId'"
        # promedioValoracion (number - int or float)
        assert "promedioValoracion" in book and isinstance(book["promedioValoracion"], (int, float)), "Missing or invalid 'promedioValoracion'"
        # totalValoraciones (int)
        assert "totalValoraciones" in book and isinstance(book["totalValoraciones"], int), "Missing or invalid 'totalValoraciones'"
        # categorias (array of strings)
        assert "categorias" in book and isinstance(book["categorias"], list), "Missing or invalid 'categorias'"
        for categoria in book["categorias"]:
            assert isinstance(categoria, str), "A 'categoria' item is not a string"

test_catalog_get_all_books()