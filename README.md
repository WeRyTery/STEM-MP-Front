# TechTrend

TechTrend is a web application for collecting, storing, and analyzing technology trends. It provides a backend API and a web interface for viewing technology data, analyzing changes, and generating recommendations.

## Technologies

- **Backend:** ASP.NET Core Web API (.NET 10)
- **Database:** PostgreSQL
- **ORM:** Entity Framework Core
- **Frontend:** React, Vite
- **Testing:** xUnit
- **CI/CD:** GitHub Actions
- **Code Coverage:** Coverlet and ReportGenerator


## Requirements

Before running the project, install:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Node.js](https://nodejs.org/) and npm
- Git

## Running the Backend

### 1. Clone the repository

### 2. Configure the database

Make sure PostgreSQL is running and create a database for the project.

Configure the database connection string using the configuration expected by the API. Do not commit passwords or other secrets to the repository.

### 3. Restore dependencies

```bash
dotnet restore TechTrend.slnx
```

### 4. Apply database migrations

```bash
dotnet ef database update --project TechTrend.Api
```

### 5. Start the API

```bash
dotnet run --project TechTrend.Api
```

The API will display its local address in the terminal. Use that address to access the available endpoints and API documentation, if enabled.

## Running the Frontend

Open a separate terminal and navigate to the directory containing the frontend's `package.json`.

Install dependencies:

```bash
npm install
```

Start the development server:

```bash
npm run dev
```

Open the local URL displayed by Vite in the terminal.

Make sure the frontend is configured to send API requests to the correct backend address.

## Running Automated Tests

From the repository root, run:

```bash
dotnet test TechTrend.Api.Tests/TechTrend.Api.Tests.csproj
```

This command builds and runs the automated tests.

## Code Coverage

To collect code coverage locally:

```bash
dotnet test TechTrend.Api.Tests/TechTrend.Api.Tests.csproj --collect:"XPlat Code Coverage"
```

GitHub Actions also runs the tests and generates an HTML coverage report. After a successful workflow run, download the `backend-coverage-report` artifact from the workflow's **Artifacts** section.

## Continuous Integration

The GitHub Actions workflow is located at:

```text
.github/workflows/backend-tests.yml
```

It restores dependencies, builds the test project, executes automated tests, generates a coverage report, and uploads the report as an artifact.

The workflow runs automatically on pushes and pull requests.