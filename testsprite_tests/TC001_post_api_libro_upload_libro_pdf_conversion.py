import requests
import io

BASE_URL = "http://localhost:5179"
TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW5AZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoiYWRtaW5AZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZWlkZW50aWZpZXIiOiIxNDExNTRmZS1kYWM0LTQyYTQtOWEyMS1lMDdhZGRiYjVkNzYiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbmlzdHJhZG9yIiwiZXhwIjoxNzgwMjU5MTQwLCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MDgwIiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzA4MCJ9.tNPFRJadEUZczha2asiQhC14lqgs95dil7Hp5Haz9Cw"
HEADERS_AUTH = {"Authorization": f"Bearer {TOKEN}"}
TIMEOUT = 30

def test_post_api_libro_upload_libro_pdf_conversion():
    uploaded_file_id = None
    created_book_id = None
    try:
        # Step 1: Upload a PDF file to /api/Libro/upload-libro (no auth required)
        pdf_content = b"%PDF-1.4\n%Dummy PDF content for testing\n"
        files = {
            "archivo": ("test_book.pdf", io.BytesIO(pdf_content), "application/pdf")
        }
        response_upload = requests.post(f"{BASE_URL}/api/Libro/upload-libro", files=files, timeout=TIMEOUT)
        assert response_upload.status_code == 200, f"Upload PDF failed: {response_upload.status_code} {response_upload.text}"
        upload_data = response_upload.json()
        # Validate that uploaded file reference is received
        assert isinstance(upload_data, dict), "Upload response is not a JSON object"
        # Try to find any string value in response dict to use as uploaded file reference
        uploaded_file_id = None
        for key, val in upload_data.items():
            if isinstance(val, str) and val.strip():
                uploaded_file_id = val
                break
        assert uploaded_file_id, "Uploaded file reference missing in response"

        # Step 2: Create book metadata referencing the uploaded file (POST /api/Libro requires auth)
        book_metadata = {
            "Titulo": "Test PDF Book",
            "Autor": "Test Author",
            "ArchivoId": uploaded_file_id
        }
        response_create = requests.post(f"{BASE_URL}/api/Libro", json=book_metadata, headers=HEADERS_AUTH, timeout=TIMEOUT)
        assert response_create.status_code == 200, f"Create book with uploaded PDF failed: {response_create.status_code} {response_create.text}"
        book_data = response_create.json()
        assert isinstance(book_data, dict), "Create book response is not a JSON object"
        created_book_id = book_data.get("id") or book_data.get("Id") or book_data.get("libroId")
        assert created_book_id, "Created book ID missing in response"

        # Verify in response that EPUB output is generated automatically
        epub_indicators = [
            "epubUrl", "epubFile", "Formato", "archivoEpub", "epubReference", "mimeType"
        ]
        epub_found = False
        for key in epub_indicators:
            val = book_data.get(key)
            if val and isinstance(val, str) and ".epub" in val.lower():
                epub_found = True
                break
        if not epub_found:
            formato = book_data.get("Formato") or book_data.get("formato")
            if formato and "epub" in formato.lower():
                epub_found = True
        assert epub_found, "PDF was not automatically converted to EPUB format upon book creation"

    finally:
        if created_book_id:
            try:
                requests.delete(f"{BASE_URL}/api/Libro/{created_book_id}", timeout=TIMEOUT)
            except Exception:
                pass


test_post_api_libro_upload_libro_pdf_conversion()
