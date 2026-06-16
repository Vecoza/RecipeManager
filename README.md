# RecipeManager

A full-stack recipe management application built with Angular and ASP.NET Core. Save recipes, manage your pantry, find recipes you can make with what you already have, and generate new ones using local AI.

> Built for my mom — because she keeps losing her recipes and I got tired of hearing about it. 💙

---

## Features

- **Recipe library** — create, browse, and view detailed recipes with ingredients and step-by-step instructions
- **Tag system** — organise recipes with custom tags
- **My Pantry** — track what ingredients you have at home
- **Recipe matching** — find exact and partial matches from your pantry
- **AI recipe generation** — generate new recipes from your pantry using a local Ollama model, with preferences for recipe type, meal type, and cooking time
- **Authentication** — JWT-based login and registration

---

## Tech Stack

| Layer | Technology |
|---|---|
| Frontend | Angular 21, Angular Material, SCSS |
| Backend | ASP.NET Core (.NET 10), EF Core 10 |
| Database | PostgreSQL |
| AI | Ollama (local LLM) |
| Auth | JWT Bearer tokens |

---

## Prerequisites

- [Node.js](https://nodejs.org/) 20+
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/) running locally
- [Ollama](https://ollama.com/) installed and a model pulled (e.g. `ollama pull llama3`)

---

## Getting Started

### 1. Database

Create a PostgreSQL database and note the connection string.

Run EF Core migrations to set up the schema:

```bash
cd src
dotnet ef database update --project RecipeManager.Infrastructure --startup-project RecipeManager.API
```

### 2. Backend configuration

Create `src/RecipeManager.API/appsettings.json` (this file is gitignored — never commit it):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=recipemanager;Username=postgres;Password=yourpassword"
  },
  "JwtSettings": {
    "Secret": "your-secret-key-at-least-64-characters-long",
    "Issuer": "RecipeManager",
    "Audience": "RecipeManagerUsers",
    "ExpiryMinutes": 60
  },
  "Ollama": {
    "BaseUrl": "http://localhost:11434",
    "Model": "llama3"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### 3. Run the backend

```bash
cd src/RecipeManager.API
dotnet run
```

The API will be available at `https://localhost:5001`.

### 4. Run Ollama (for AI generation)

```bash
ollama serve
```

Make sure the model you configured is pulled:

```bash
ollama pull llama3
```

### 5. Run the frontend

```bash
cd recipe-manager-ui
npm install
npm start
```

Open [http://localhost:4200](http://localhost:4200) in your browser.

---

## Project Structure

```
RecipeManager/
├── src/
│   ├── RecipeManager.API/           # ASP.NET Core Web API (controllers, auth, DI)
│   └── RecipeManager.Infrastructure/ # EF Core, services, repositories, DTOs
└── recipe-manager-ui/               # Angular 21 frontend
    └── src/app/
        ├── core/                    # Services, guards, interceptors
        ├── features/                # Pages: auth, recipes, pantry
        └── shared/                  # Models, reusable components
```

---

## Notes

- The first AI generation can take 2–3 minutes on CPU — this is normal.
- `appsettings.json` is gitignored. Never commit it — it contains your database password and JWT secret.
