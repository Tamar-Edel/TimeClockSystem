# TimeClockSystem

A full-stack employee attendance system built with **ASP.NET Core 7** and **React 19**. Employees register an account, log in, and clock in or out of shifts through a clean web interface. All timestamps are sourced from an external time API — local system time is never used.

Built as a home assignment for a Junior Full Stack Developer position.

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Authentication Flow](#authentication-flow)
- [External Time Source](#external-time-source)
- [API Reference](#api-reference)
- [Setup & Running](#setup--running)
- [Demo Credentials](#demo-credentials)
- [Key Engineering Decisions](#key-engineering-decisions)
- [Limitations](#limitations)
- [Future Improvements](#future-improvements)

---

## Overview

TimeClockSystem lets employees track their working hours through a browser. Each employee can clock in to start a shift and clock out to end it. Shift history is available on the dashboard, showing clock-in time, clock-out time, and calculated duration.

The system enforces that only one shift can be open at a time per employee — both in application logic and at the database level.

---

## Features

- **User registration and login** with JWT-based authentication
- **Clock In / Clock Out** — one open shift per user at a time
- **Shift history** — full list of past and current shifts with duration
- **Dashboard** showing current shift status and shift log
- **Protected routes** — unauthenticated users are redirected to login
- **Automatic logout** on expired or invalid token (401 response)
- **Demo account** pre-seeded on first run
- **Swagger UI** available for backend API exploration

---

## Architecture

The backend follows a three-layer clean architecture with a strict one-direction dependency rule:

```
API  →  Core  →  Infrastructure
```

- **Core** contains entities, interfaces, DTOs, and services. It has no external dependencies.
- **Infrastructure** implements the interfaces defined in Core: repositories, database context, JWT service, password hasher, and the external time provider.
- **API** wires everything together via dependency injection and exposes HTTP endpoints through controllers.

**Project layout:**

```
TimeClockSystem/
├── server/
│   ├── TimeClockSystem.API/           # Controllers, middleware, Program.cs
│   ├── TimeClockSystem.Core/          # Entities, interfaces, DTOs, services
│   └── TimeClockSystem.Infrastructure/ # EF Core, repositories, JWT, BCrypt, time API
│
├── client/
│   └── time-clock-client/             # React 19 + Vite frontend
│
├── TimeClockSystem.sln
└── README.md
```

**Frontend layout:**

```
src/
├── api/          # Axios client, auth API, shifts API
├── components/   # ClockButtons, CurrentShiftCard, ShiftTable, PrivateRoute
├── context/      # AuthContext — global auth state
├── pages/        # LoginPage, RegisterPage, DashboardPage, NotFoundPage
└── utils/        # Date formatters, localStorage helpers
```

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend framework | ASP.NET Core 7 Web API |
| ORM | Entity Framework Core 7 |
| Database | SQL Server (LocalDB in development) |
| Authentication | JWT Bearer (HS256, 8-hour expiry) |
| Password hashing | BCrypt.Net-Next |
| External time | timeapi.io |
| API docs | Swagger / Swashbuckle |
| Frontend framework | React 19 |
| Build tool | Vite 7 |
| Routing | React Router DOM 7 |
| HTTP client | Axios |
| State management | React Context API |

---

## Authentication Flow

1. User submits credentials via the login or register form.
2. The API validates the request, hashes/verifies the password with BCrypt, and returns a signed JWT.
3. The frontend stores the token in `localStorage` and attaches it as a `Bearer` header on every subsequent request via an Axios request interceptor.
4. On app load, if a token exists in storage, the frontend calls `GET /api/auth/me` to restore user state. If that request fails, the token is discarded and the user is redirected to login.
5. A response interceptor catches any 401 response (outside of auth endpoints) and triggers automatic logout.
6. Protected routes are guarded by a `PrivateRoute` component that checks the auth context before rendering.

---

## External Time Source

Clock In and Clock Out timestamps are always fetched from:

```
https://timeapi.io/api/v1/time/current/zone?timezone=Europe/Zurich
```

The response field `date_time` is parsed as `DateTimeOffset`, which includes the Zurich UTC offset (+01:00 or +02:00 depending on DST).

**Rules enforced in code:**
- `DateTime.Now` and `DateTime.UtcNow` are never used for shift times.
- If the external API call fails for any reason, the clock-in or clock-out operation fails with an error.
- There is no fallback to local system time.

The time source is also recorded on each shift row (`ClockInTimeSource`, `ClockOutTimeSource`) as an audit field.

---

## API Reference

All shift endpoints require a valid JWT in the `Authorization: Bearer <token>` header.

### Auth

| Method | Endpoint | Body | Description |
|--------|----------|------|-------------|
| POST | `/api/auth/register` | `{ fullName, email, password }` | Create account, returns JWT |
| POST | `/api/auth/login` | `{ email, password }` | Login, returns JWT |
| GET | `/api/auth/me` | — | Returns logged-in user info |

### Shifts

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/shifts/clock-in` | Start a new shift |
| POST | `/api/shifts/clock-out` | End the current open shift |
| GET | `/api/shifts/current` | Get the currently open shift, if any |
| GET | `/api/shifts/history` | Get all shifts, ordered by clock-in time descending |

**Error responses** use a consistent JSON format:

```json
{ "message": "User already has an open shift." }
```

Business rule violations return `400 Bad Request`. Unexpected errors return `500 Internal Server Error`.

---

## Setup & Running

### Prerequisites

- [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [SQL Server LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) (ships with Visual Studio)
- [Node.js 18+](https://nodejs.org/) and npm

---

### Backend

```bash
cd server/TimeClockSystem.API
dotnet restore
dotnet run
```

The API starts on `http://localhost:5113`.

On first run it will:
1. Apply EF Core migrations automatically.
2. Seed a demo user account.

Swagger UI is available at `http://localhost:5113/swagger`.

**Connection string** (configured in `appsettings.Development.json`):
```
Server=(localdb)\mssqllocaldb;Database=TimeClockDb;Trusted_Connection=True;
```

To run migrations manually:
```bash
cd server/TimeClockSystem.API
dotnet ef database update
```

---

### Frontend

```bash
cd client/time-clock-client
npm install
npm run dev
```

The app starts on `http://localhost:5173`.

The Axios client is pre-configured to call `http://localhost:5113/api`. No `.env` file is needed for local development.

---

## Demo Credentials

A demo account is seeded automatically on first run:

| Field | Value |
|-------|-------|
| Email | demo@timeclock.com |
| Password | Demo1234! |

---

## Key Engineering Decisions

**External-only time source**
Using `DateTime.Now` would tie shift records to the server's local clock, which can drift, be misconfigured, or vary across environments. An external API with a fixed timezone eliminates that variability and provides an auditable, consistent time source. If the API is unreachable, the operation fails rather than silently recording an untrustworthy time.

**Double enforcement of the one-open-shift rule**
The business rule "a user cannot have more than one open shift" is enforced in two places:
1. `ShiftService` checks for an existing open shift before inserting.
2. A filtered unique index on `(UserId) WHERE ClockOutAt IS NULL` in the database prevents duplicate open shifts at the storage level, protecting against race conditions.

**Clean architecture with explicit dependency direction**
Core contains no references to EF Core, SQL Server, or any infrastructure concern. This keeps business logic independently testable and makes the infrastructure interchangeable. The direction `API → Core → Infrastructure` is enforced by the project references in each `.csproj`.

**Explicit DTOs between layers**
Controllers never expose entity classes directly. Each operation has a dedicated request or response DTO containing only the fields that belong in that context. This keeps the API contract stable even if the domain model changes.

**JWT stored in localStorage**
For the scope of this project, `localStorage` is used for token persistence. It is simple and sufficient for a controlled environment. In a production application, `HttpOnly` cookies would be preferable to mitigate XSS exposure.

---

## Limitations

- No token refresh — the user must log in again after the 8-hour JWT expires.
- No admin panel or user management — all users are self-registered.
- The JWT secret is stored in `appsettings.json` and must be replaced with a secret manager or environment variable before any real deployment.
- Timezone is fixed to `Europe/Zurich` — not configurable per user.
- No pagination on shift history.
- No HTTPS enforcement in development configuration.

---

## Future Improvements

- Refresh token rotation for persistent sessions
- Role-based access (admin role with user management and full shift overview)
- Configurable timezone per user or organization
- Pagination and date filtering on shift history
- Export shift data to CSV
- Move JWT secret to environment variables / secrets management
- Containerize with Docker and Docker Compose for easier local setup
