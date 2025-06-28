# TripNest

TripNest is a travel and tour management web application built with ASP.NET Core and Entity Framework Core. It allows users to browse tours, make bookings, manage user profiles, and interact with travel agencies.

## Features
- User registration and authentication
- Tour listing and booking
- Agency management
- User profile management
- Reviews and contact messages

## Project Structure
- `Controllers/` - MVC controllers for handling HTTP requests
- `Models/` - Entity models for the application
- `Data/` - Database context and migrations
- `Views/` - Razor views for UI
- `wwwroot/` - Static files (CSS, JS, images)

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (or update connection string for your DB)

### Setup
1. Clone the repository:
   ```sh
   git clone <repository-url>
   cd TripNest
   ```
2. Restore dependencies:
   ```sh
   dotnet restore
   ```
3. Apply migrations and update the database:
   ```sh
   dotnet ef database update
   ```
4. Run the application:
   ```sh
   dotnet run
   ```
5. Open your browser and navigate to `https://localhost:5001` (or the URL shown in the console).

## Configuration
- Update `appsettings.json` and `appsettings.Development.json` for your database connection and other settings.

## License
This project is licensed under the MIT License.
