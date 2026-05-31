import requests

BASE_URL = "http://localhost:5179"
TIMEOUT = 30

def test_epub_get_manifest():
    try:
        # Step 1: Get a book ID using paged catalog API
        paged_url = f"{BASE_URL}/api/Libro/paged"
        params = {"page": 1, "pageSize": 1}
        response = requests.get(paged_url, params=params, timeout=TIMEOUT)
        assert response.status_code == 200, f"Expected 200 from paged books, got {response.status_code}"
        paged_data = response.json()
        assert "data" in paged_data and isinstance(paged_data["data"], list), "Missing or invalid 'data' field in paged response"
        assert len(paged_data["data"]) > 0, "No books found in paged data"
        libro_id = paged_data["data"][0].get("id")
        assert isinstance(libro_id, int), "Libro ID is not an integer"

        # Step 2: Get EPUB manifest for the obtained libroId
        manifest_url = f"{BASE_URL}/api/epub/{libro_id}/manifest"
        manifest_response = requests.get(manifest_url, timeout=TIMEOUT)

        # Assert status code is 200 or 404
        assert manifest_response.status_code in [200, 404], f"Manifest response status_code not in [200, 404], got {manifest_response.status_code}"

        if manifest_response.status_code == 200:
            manifest_data = manifest_response.json()
            # Assert required fields matching PRD with correct casing
            assert "Title" in manifest_data, "'Title' field missing in manifest response"
            assert "Author" in manifest_data, "'Author' field missing in manifest response"

            # Validate readingOrder field is a list with items containing Href, Type, MediaType (PascalCase)
            assert "ReadingOrder" in manifest_data, "'ReadingOrder' field missing in manifest response"
            reading_order = manifest_data["ReadingOrder"]
            assert isinstance(reading_order, list), "'ReadingOrder' is not a list"
            for item in reading_order:
                assert isinstance(item, dict), "ReadingOrder item is not a dict"
                assert "Href" in item, "ReadingOrder item missing 'Href'"
                assert "Type" in item, "ReadingOrder item missing 'Type'"
                assert "MediaType" in item, "ReadingOrder item missing 'MediaType'"

            # Validate resources field is a list
            assert "Resources" in manifest_data, "'Resources' field missing in manifest response"
            resources = manifest_data["Resources"]
            assert isinstance(resources, list), "'Resources' is not a list"

    except requests.RequestException as e:
        assert False, f"RequestException occurred: {e}"
    except AssertionError as ae:
        raise ae

test_epub_get_manifest()
