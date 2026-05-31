# OpenBook - Backend Mobile

## Purpose
Backend API for a mobile e-book reader application that allows users to browse a catalog, read EPUB books, manage their personal library, and interact with books through bookmarks, highlights, ratings, and reviews.

## Core Features

### 1. Authentication & User Management
- User registration with email/password
- JWT-based login
- Password recovery flow
- Profile management (name, photo, email)
- Two roles: Administrator and Usuario

### 2. Book Catalog
- Public catalog with pagination
- Search by title, author, description
- Filter by category
- Book metadata: title, author, description, cover image
- PDF to EPUB automatic conversion on upload

### 3. EPUB Reader Engine
- Serve EPUB manifest (reading order, resources, table of contents)
- Serve individual EPUB resources (chapters, images, CSS) with correct MIME types
- Support for standard EPUB format using VersOne.Epub library

### 4. User Library
- Add/remove books to personal library
- Track reading progress (% and current page)
- Mark books as favorites
- Library sorted by last read date

### 5. Bookmarks
- Create bookmarks at specific pages
- List bookmarks by user or by book
- Update bookmark page number
- Delete bookmark (owner only)

### 6. Highlights (Resaltadores)
- Create highlights with CFI range, href, and color
- List highlights by user or by book
- Update highlight content and color
- Delete highlight (owner only)

### 7. Ratings (Valoraciones)
- Rate books on 1-5 scale
- One rating per user per book (unique constraint)
- Update or delete own rating
- View all ratings for a book
- Top 5 books by average rating

### 8. Reviews (Reseñas)
- Write text reviews for books
- Edit or delete own review
- View all reviews for a book (ordered by date)

### 9. Categories
- Create, update, delete categories
- Assign categories to books (many-to-many)
- List categories with book count

### 10. Moderation
- User suggestions/feedback system
- User reporting system (denuncias)
- Admin management for suggestions and reports

## Technical Requirements
- ASP.NET Core 8 Web API
- PostgreSQL database
- Entity Framework Core for ORM
- JWT Bearer authentication
- RESTful API design
- Swagger documentation
- File upload support (books, covers, profile photos)
