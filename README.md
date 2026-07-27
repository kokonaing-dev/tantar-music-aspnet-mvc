# TANTAR Music

A Spotify-inspired music streaming web application built with ASP.NET Core 9 MVC.

## Features

- Browse and play songs, albums, and artists
- User registration and login with role-based access control
- Admin panel for uploading and managing music content
- Personal playlist creation and management
- Dark-themed UI inspired by Spotify (Bootstrap 5 + Bootstrap Icons)

## Roles

| Role  | Permissions |
|-------|-------------|
| Admin | Upload songs, manage artists, albums, songs |
| User  | Register/login, create and manage own playlists |

Default admin account seeded on startup:
- Email: `admin@tantar.com`
- Password: `Admin123!`

## Tech Stack

- **Framework:** ASP.NET Core 9 MVC
- **ORM:** Entity Framework Core 9
- **Database:** SQL Server
- **Auth:** ASP.NET Core Identity
- **Frontend:** Bootstrap 5, Bootstrap Icons, Razor Views

## Architecture

```
Controllers/   — thin controllers delegating to services
Services/      — business logic layer
Repositories/  — data access layer
Models/
  Domain/      — EF Core entities (Song, Album, Artist, Playlist, ApplicationUser)
  ViewModels/  — view-specific models
Views/         — Razor views
wwwroot/
  uploads/
    audio/     — song audio files
    covers/    — album/song cover images
    profiles/  — user profile pictures
```

## Getting Started

### Prerequisites

- .NET 9 SDK
- SQL Server

### Setup

1. Clone the repository:
   ```bash
   git clone <repo-url>
   cd TANTAR_Music
   ```

2. Update the connection string in `appsettings.json` if needed:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=.;Database=TantarMusic;User ID=sa;Password=...;TrustServerCertificate=True;"
   }
   ```

3. Apply migrations and seed the database:
   ```bash
   cd TANTAR_Music
   dotnet ef database update
   ```

4. Run the application:
   ```bash
   dotnet run
   ```

The app will be available at `https://localhost:5001`.