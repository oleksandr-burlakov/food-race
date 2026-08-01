# General Overview of Backend

This is the backend implementation of the **'food-race'** project. For now, we have a Modular Monolith implementation that eventually will be evolved into separate microservices. This is a learning project aimed at mastering modern .NET concepts, software architecture patterns, and backend infrastructure.

Basically, this backend will be divided into four core modules and a shared kernel:

- **Module.Authentication**: Responsible for user registration, authentication, authorization, and token management (JWT/Identity).
- **Module.Catalog**: Catalog of restaurants, categories, and available dishes.
  - _Tech Stack_: MongoDB, Minimal APIs / Carter.
- **Module.Orders**: Responsible for order creation, order items, status workflow, and customer relationships.
  - _Tech Stack_: PostgreSQL, Entity Framework Core / Dapper, MediatR.
- **Module.Delivery**: Responsible for delivery execution, managing couriers, delivery jobs, and real-time courier location.
  - _Tech Stack_: PostgreSQL, Entity Framework Core, MediatR.
- **Shared**: Class library containing core infrastructure and abstractions shared across modules (Result Pattern, Global Filters, Helper Extensions, Base Domain Classes, Bus interfaces, and infrastructure contracts).

## Communication Rules Across Modules

- Direct hard dependencies between modules are prohibited.
- Modules must interact only through:
  1. An in-memory/external **Event Bus** (Pub/Sub pattern).
  2. Abstracted **Interfaces/Contracts** placed in `Shared` or contract assemblies (which will be replaced with HTTP/gRPC calls once split into microservices).

---

## Architectural Conventions & Infrastructure

### 1. Endpoint Routing

- We use Carter (built on top of .NET Minimal APIs) to encapsulate routes cleanly into modular, single-responsibility modules instead of monolithic controllers.

### 2. Structured Logging & Observability

- Serilog is used as the primary logging framework across all modules.

### 3. Feature Flagging & Dynamic Toggles

- Unleash is integrated for runtime feature toggling (e.g., canary releases, enabling/disabling endpoints).
- Protected endpoints use a custom `UnleashFeatureFilter` (`IEndpointFilter`) attached natively via Carter routes (`.RequireFeatureFlag("flag-name")`).

---

## Technical Stack & Libraries (TODO List)

Below is the list of packages and tools used/planned for integration:

### Core Frameworks & Routing

- [x] **Carter** — Clean Minimal API module organization
- [x] **Serilog.AspNetCore** — Structured logging framework (with Console & File sinks)
- [x] **Unleash.FeatureToggle.Client** — Feature flag management

### CQRS & Messaging

- [ ] **MediatR** / **MediatR.Contracts** — In-process messaging and CQRS implementation
- [ ] MassTransit / RabbitMQ (Optional/Future) — External event bus

### Data Access & Persistence

- [x] **Npgsql.EntityFrameworkCore.PostgreSQL** — EF Core provider for PostgreSQL (Orders, Delivery)
- [ ] **MongoDB.Driver** / EF Core Mongo Provider — Database driver for Catalog
- [ ] **Dapper** — High-performance Micro-ORM for complex/read-heavy queries

---

## Future Module Readmes

Each module directory (`/src/Modules/Module.*`) will eventually contain its own dedicated `README.md` detailing:

- Domain entities & Aggregate Roots
- Module-specific commands, queries, and published domain/integration events
- Local configuration & persistence setup
