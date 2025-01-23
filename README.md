# LuxMed

This README contains information on how to set up and run the backend .NET application.

## Requirements

Before starting, ensure you have the following installed:

- **.NET 8 SDK**

## Development Setup

1. **Clone the Repository**

   ```bash
   git clone <repository-url>
   cd LuxMedAPI
   ```

2. **Configure Environment**

   Create or update the `appsettings.json` file to configure settings.
   You can copy `appsettings.Develpment.json` file to configure settings => `appsettings.json`
   
3. **Run the Application**

   Start the application in development mode:

   ```bash
   dotnet run
   ```

   The backend API will be available at [http://localhost:5000](http://localhost:5000).

4. **Database Migration**

   Ensure the database is up to date by applying migrations:

   ```bash
   dotnet ef database update
   ```
   Database is applying migrations also automatically:

## Authentication

The backend requires login credentials to access secured endpoints. Use the following default credentials:

- **Username:** admin
- **Password:** admin123

Authentication is managed using cookies.

## Notes

- Ensure the frontend is configured to connect to the correct backend URL (`http://localhost:5000` during development).
- Use `dotnet publish` for production builds.
