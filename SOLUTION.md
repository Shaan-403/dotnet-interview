# Solution Documentation

**Candidate Name:** Shantanu Sahu  
**Completion Date:** 08 July 2026

---

# Problems Identified

After reviewing the original implementation, I identified several areas for improvement.

### Architecture & Design

- Controller and service were tightly coupled.
- The controller directly depended on the concrete `TodoService` implementation instead of an abstraction.
- Request models reused the database entity (`Todo`) instead of dedicated API models.
- Database configuration was partially hardcoded.

### Code Quality & Maintainability

- Duplicate object mapping logic existed in multiple methods.
- Configuration was not centralized.
- Nullable reference warnings were present.
- The service returned manually constructed objects instead of retrieving persisted data.

### Security

- SQL statements were built using string interpolation, making the application vulnerable to SQL Injection.

### Functional Bug

- Newly created TODOs always returned `Id = 0` even though SQLite generated a valid primary key.

### Testing

- Existing tests depended on the previous implementation and no longer worked after introducing dependency injection.
- Tests relied on hardcoded construction of services instead of using configuration.

---

# Architectural Decisions

The primary goal was to improve maintainability, security and testability while preserving the existing API behaviour.

## Dependency Injection

Introduced `ITodoService` and registered the service through ASP.NET Core Dependency Injection.

Benefits:

- Loose coupling
- Easier testing
- Better separation of concerns

---

## DTOs

Created dedicated request DTOs for:

- CreateTodo
- UpdateTodo
- GetTodo
- DeleteTodo

This prevents clients from sending properties such as:

- Id
- CreatedAt
- IsCompleted

that should be controlled by the server.

---

## Configuration

Moved the SQLite connection string into `appsettings.json`.

The service now retrieves it using `IConfiguration`, making configuration environment-independent.

---

## Database Access

Refactored all SQL statements to use parameterized queries.

Benefits:

- Prevents SQL Injection
- Cleaner SQL
- Easier maintenance

---

## Service Improvements

Improved the service layer by:

- Extracting duplicate mapping logic into a helper method.
- Returning nullable values where appropriate.
- Returning the persisted entity after updates.
- Correctly retrieving SQLite generated IDs using `last_insert_rowid()`.

---

# Trade-offs

Given the limited time available, I focused on improvements that provided the highest value while minimizing risk.

### Prioritized

- Security
- Dependency Injection
- Configuration
- DTOs
- Bug fixes
- Test compatibility

### Deferred

I intentionally did not introduce:

- Entity Framework Core
- Repository Pattern
- AutoMapper
- Async database operations
- Global exception middleware

Although these would further improve the project, they would significantly increase the scope and introduce unnecessary complexity for a small CRUD application.

### Backward Compatibility

I intentionally preserved the existing API endpoints and HTTP methods to maintain backward compatibility with the existing application behaviour.

Although a RESTful API would typically expose endpoints such as:

- `GET /todos/{id}`
- `POST /todos`
- `PUT /todos/{id}` or `PATCH /todos/{id}`
- `DELETE /todos/{id}`

changing the existing routes (for example `POST /api/createTodo` and `POST /api/getTodo`) would introduce breaking API changes.

Since the objective of the assignment was to review and improve the existing implementation rather than redesign the public API, I chose to preserve the current API contract while improving the internal architecture, security, maintainability, and testability.

In a greenfield project, I would redesign the API to follow REST conventions with resource-based URLs and appropriate HTTP verbs.
---

# How to Run

## Prerequisites

- .NET SDK 8.0
- SQLite (database is created automatically if it does not exist)

---

## Build

```bash
dotnet build
```

## Run

```bash
cd TodoApi
dotnet run
```

Swagger is available at:

```
http://localhost:5164/swagger
```

(Port may vary depending on local configuration.)

---

## Test

```bash
dotnet test
```

---

# API Documentation

## Create TODO

```
Method:
POST

URL:
/api/createTodo
```

### Request

```json
{
  "title": "Buy groceries",
  "description": "Milk and eggs"
}
```

### Response

```json
{
  "id": 1,
  "title": "Buy groceries",
  "description": "Milk and eggs",
  "isCompleted": false,
  "createdAt": "2026-07-08T10:30:00Z"
}
```

---

## Get TODO(s)

```
Method:
POST

URL:
/api/getTodo
```

### Get All

```json
{}
```

### Get By Id

```json
{
  "id": 1
}
```

---

## Update TODO

```
Method:
POST

URL:
/api/updateTodo
```

### Request

```json
{
  "id": 1,
  "title": "Updated title",
  "description": "Updated description",
  "isCompleted": true
}
```

---

## Delete TODO

```
Method:
POST

URL:
/api/deleteTodo
```

### Request

```json
{
  "id": 1
}
```

### Response

```json
{
  "message": "Todo deleted successfully"
}
```

---

# Future Improvements

If more time were available, I would further enhance the project by:

- Implementing global exception handling middleware.
- Introducing asynchronous database operations.
- Adding structured logging using `ILogger`.
- Improving test isolation with a dedicated test database.
- Increasing test coverage using mocking for controller unit tests.
- Introducing API versioning if the application grows.
- Adding pagination and filtering for larger TODO collections.
- Containerizing the application using Docker.
- Adding a CI pipeline for automated build and test execution.