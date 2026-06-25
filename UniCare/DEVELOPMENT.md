# UniCare Backend Development Setup Guide

This document provides step-by-step instructions to configure, initialize, and run the UniCare backend services locally for development.

---

## Prerequisites

Before setting up the project, ensure you have the following software installed:

1. **.NET 8 SDK** (v8.0 or higher)
2. **Microsoft SQL Server** (LocalDB, Express, or Developer Edition)
3. **SQL Server Management Studio (SSMS)** or Azure Data Studio
4. **Entity Framework Core CLI Tools**
   * To install globally, execute: `dotnet tool install --global dotnet-ef`

---

## Configuration and Environment Setup

The application looks for database credentials and service settings in `appsettings.json` located within the `UniCare.Api` project directory.

### Database Connection Strings
Modify the connection string inside `UniCare.Api/appsettings.json` to point to your local SQL Server instance:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=UniCareDB;Trusted_Connection=True;TrustServerCertificate=True"
}
```

* **For SQL Express instances:** Use `Server=.\\SQLEXPRESS;...`
* **For LocalDB instances:** Use `Server=(localdb)\\mssqllocaldb;...`
* **For Default MSSQL instances:** Use `Server=.;...` (Uses Windows Authentication)

### External Services (Cloudinary)
To enable multipart image uploads to Cloudinary during local development, populate the `CloudinarySettings` section:

```json
"CloudinarySettings": {
  "CloudName": "YOUR_CLOUD_NAME",
  "ApiKey": "YOUR_API_KEY",
  "ApiSecret": "YOUR_API_SECRET"
}
```

---

## Database Initialization and Migrations

The database schema is managed using Entity Framework Core Code-First migrations. Follow these instructions to apply the migrations and generate the database:

1. Open a terminal at the solution root folder (`k:\UniCare\UniCare`).
2. Run the database update command to apply existing migrations:
   ```bash
   dotnet ef database update --project UniCare.Infrastructure --startup-project UniCare.Api
   ```

*Note: If you make changes to the domain models, generate a new migration by running:*
```bash
dotnet ef migrations add MigrationName --project UniCare.Infrastructure --startup-project UniCare.Api
```

---

## Running the Application

You can start the API using either the command-line interface or Visual Studio.

### Using .NET CLI
1. Navigate to the API presentation directory:
   ```bash
   cd UniCare.Api
   ```
2. Start the development server:
   ```bash
   dotnet run
   ```
3. Observe the console logs for the active listening ports (typically `http://localhost:5111` or `https://localhost:7111`).

### Using Visual Studio
1. Open the `UniCare.sln` solution file in Visual Studio.
2. Right-click the `UniCare.Api` project and select **Set as Startup Project**.
3. Press **F5** or click **Start Debugging** to build and launch the application.

---

## API Verification and Interactive Testing

Once the application is running:
1. Open your web browser and navigate to:
   ```
   http://localhost:5111/swagger/index.html
   ```
2. The Swagger UI dashboard will display all available HTTP REST endpoints (such as `/api/v1/Items` and `/api/auth/login`).
3. You can use the built-in "Try it out" feature to execute requests and verify correct database persistence.
