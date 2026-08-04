<p align="center">
  <img src="https://img.shields.io/badge/ClinicOS-Clinic_Management_System-0078D4?style=for-the-badge&logo=dotnet&logoColor=white" alt="ClinicOS" />
</p>

<h1 align="center">🏥 ClinicOS</h1>

<p align="center">
  <strong>A Comprehensive, Enterprise-Grade Clinic Management System</strong><br/>
  <em>Built with .NET 10 · Clean Architecture · CQRS · Domain-Driven Design</em>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet&logoColor=white" alt=".NET 10" />
  <img src="https://img.shields.io/badge/C%23-13-239120?style=flat-square&logo=csharp&logoColor=white" alt="C# 13" />
  <img src="https://img.shields.io/badge/EF_Core-10.0-purple?style=flat-square&logo=nuget&logoColor=white" alt="EF Core 10" />
  <img src="https://img.shields.io/badge/Docker-Ready-2496ED?style=flat-square&logo=docker&logoColor=white" alt="Docker" />
  <img src="https://img.shields.io/badge/License-MIT-green?style=flat-square" alt="License" />
  <img src="https://img.shields.io/badge/Architecture-Clean_Architecture-FF6F00?style=flat-square" alt="Architecture" />
  <img src="https://img.shields.io/badge/Pattern-CQRS-E91E63?style=flat-square" alt="CQRS" />
</p>

<p align="center">
  <a href="#-features">Features</a> •
  <a href="#-architecture">Architecture</a> •
  <a href="#-tech-stack">Tech Stack</a> •
  <a href="#-getting-started">Getting Started</a> •
  <a href="#-project-structure">Project Structure</a> •
  <a href="#-roadmap">Roadmap</a> •
  <a href="#-contributing">Contributing</a>
</p>

---

## 📖 Overview

**ClinicOS** is a production-ready, **single-tenant** clinic management system designed to power the operations of a single medical facility with the robustness and scalability of enterprise software. From patient registration and staff management to appointment scheduling and medical records — ClinicOS delivers a unified platform built on battle-tested .NET best practices.

> **🎯 Design Philosophy:** While ClinicOS is built for a single clinic deployment (not SaaS), its architecture follows enterprise-grade patterns — Clean Architecture, CQRS, and DDD — ensuring the codebase remains maintainable, testable, and extensible as your clinic grows.

<p align="center">
  <em>
    نظام إدارة عيادات/مراكز طبية شامل ومتكامل، مبني بأعلى معايير هندسة البرمجيات ومخصص لإدارة منشأة طبية واحدة بكفاءة مؤسسية.
  </em>
</p>

---

## ✨ Features

### ✅ Completed Modules

<table>
  <tr>
    <td width="50%">

#### 🔐 Identity & Account Management
- Multi-strategy authentication (**Email/Password**, **OTP**, **Google OAuth**)
- **Role-based** & **Permission-based** authorization system
- Assign / revoke roles and granular permissions per user
- Professional **HTML email templates** dispatched via background jobs
- Secure **JWT Token** management with refresh support

</td>
    <td width="50%">

#### 👨‍⚕️ Staff Management
- Full lifecycle management for **Doctors** and **Receptionists**
- Independent registration pipelines (separate from patient flows)
- **Invitation-based onboarding** for new staff members
- Self-service **profile management** (view / update own profile)
- Staff-specific data models and validation rules

</td>
  </tr>
  <tr>
    <td width="50%">

#### 🧑‍🤝‍🧑 Patient Management
- **Dual-path smart registration:**
  - 🔹 **Self-registration** — Patients sign up via OTP or Google OAuth
  - 🔹 **Reception-assisted** — Receptionist registers the patient directly (no OTP required)
- Personal account portal: **"My Profile"** (Get / Update)
- Complete patient CRUD with advanced search & pagination

</td>
    <td width="50%">

#### 🩺 Specializations & Scheduling
- Full CRUD for medical **Specializations**
- Doctor **schedule & availability** management
- Linked to doctors for specialty-based lookups
- **Intelligent caching** with automatic invalidation on data changes
- Paginated and filterable API endpoints

</td>
  </tr>
</table>

### 🔧 Cross-Cutting Concerns (Built-In)

| Concern | Implementation | Description |
|:--------|:---------------|:------------|
| 🔄 **Caching** | `ICacheableQuery` / `ICacheInvalidatorCommand` | Read-through cache with automatic prefix-based invalidation on writes |
| 📋 **Validation** | FluentValidation Pipeline Behavior | Automatic request validation before handler execution |
| 🔒 **Authorization** | Custom Authorization Behavior | Permission checks enforced at the pipeline level |
| 📊 **Logging** | Serilog + Structured Logging Behavior | Rich contextual logging with correlation IDs |
| ⚡ **Performance** | Performance Monitoring Behavior | Automatic slow-query detection and alerting |
| 🔀 **Transactions** | Transaction Behavior Pipeline | Automatic unit-of-work scoping for commands |
| 🌐 **Correlation** | Correlation ID Middleware | End-to-end request tracing across the pipeline |
| 🛡️ **Error Handling** | Global Exception Handler + Result Pattern | Unified, consistent error responses across all endpoints |
| 📨 **Background Jobs** | Hangfire (`IJobScheduler`) | Async email delivery, notifications, and outbox processing |
| 📤 **Outbox Pattern** | `ProcessOutboxMessagesJob` | Reliable domain event publishing with guaranteed delivery |

---

## 🏗️ Architecture

ClinicOS follows **Clean Architecture** with strict dependency inversion, ensuring the domain and business logic remain completely isolated from infrastructure concerns.

```
┌──────────────────────────────────────────────────────────┐
│                     🌐 API Layer                         │
│         Controllers · Middlewares · Contracts            │
│              (ClinicOS.Api)                               │
├──────────────────────────────────────────────────────────┤
│                  📦 Application Layer                    │
│     CQRS Handlers · Validators · Pipeline Behaviors     │
│    Abstractions · Pagination · Mapping Profiles          │
│           (ClinicOS.Application)                         │
├──────────────────────────────────────────────────────────┤
│                  🏛️ Domain Layer                         │
│      Entities · Value Objects · Domain Events            │
│       Enums · Constants · Error Definitions              │
│             (ClinicOS.Domain)                             │
├──────────────────────────────────────────────────────────┤
│                🔧 Infrastructure Layer                   │
│   EF Core · Identity · Email · Caching · Jobs · Auth    │
│    File Storage · External Services · Persistence        │
│          (ClinicOS.Infrastructure)                       │
└──────────────────────────────────────────────────────────┘
```

### 🎯 Key Architectural Patterns

| Pattern | Purpose | Implementation |
|:--------|:--------|:---------------|
| **Clean Architecture** | Strict layer separation with dependency inversion | 4-layer project structure with unidirectional dependencies |
| **CQRS** | Segregation of read/write operations | MediatR-based Commands & Queries with dedicated handlers |
| **Domain-Driven Design** | Rich domain modeling | Entities, Value Objects, Domain Events, Aggregate Roots |
| **Result Pattern** | Explicit, type-safe error handling | `Result<T>` monad replacing exceptions for flow control |
| **Repository / DbContext Abstraction** | Data access isolation | `IApplicationDbContext` — zero EF Core dependency in Application layer |
| **Outbox Pattern** | Reliable event publishing | Background job processes outbox messages for guaranteed delivery |
| **MediatR Pipelines** | AOP-style cross-cutting concerns | Validation → Authorization → Caching → Logging → Transaction |
| **Options Pattern** | Strongly-typed configuration | `IOptions<T>` for all external service configuration |

### 🔄 Request Pipeline Flow

```
HTTP Request
    │
    ▼
┌─────────────────────┐
│  Correlation ID MW   │  ← Assigns trace ID
├─────────────────────┤
│  Global Exception    │  ← Catches unhandled errors
│  Handler             │
├─────────────────────┤
│  Controller          │  ← Maps HTTP → MediatR
├─────────────────────┤
│  Validation Behavior │  ← FluentValidation
├─────────────────────┤
│  Authorization       │  ← Permission checks
│  Behavior            │
├─────────────────────┤
│  Caching Behavior    │  ← Cache hit? Return cached
├─────────────────────┤
│  Logging Behavior    │  ← Structured logging
├─────────────────────┤
│  Performance         │  ← Slow query detection
│  Behavior            │
├─────────────────────┤
│  Transaction         │  ← Unit of Work scope
│  Behavior            │
├─────────────────────┤
│  Command / Query     │  ← Business logic execution
│  Handler             │
└─────────────────────┘
    │
    ▼
HTTP Response (Result<T>)
```

---

## 🛠️ Tech Stack

<table>
  <thead>
    <tr>
      <th align="center">Category</th>
      <th align="center">Technology</th>
      <th align="center">Version</th>
      <th align="center">Purpose</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td align="center">⚙️ <strong>Runtime</strong></td>
      <td>.NET</td>
      <td><code>10.0</code></td>
      <td>Latest LTS framework</td>
    </tr>
    <tr>
      <td align="center">🗄️ <strong>ORM</strong></td>
      <td>Entity Framework Core</td>
      <td><code>10.0</code></td>
      <td>Data access & migrations</td>
    </tr>
    <tr>
      <td align="center">📬 <strong>Mediator</strong></td>
      <td>MediatR</td>
      <td><code>12.3</code></td>
      <td>CQRS command/query dispatching</td>
    </tr>
    <tr>
      <td align="center">✅ <strong>Validation</strong></td>
      <td>FluentValidation</td>
      <td><code>12.1</code></td>
      <td>Request validation pipeline</td>
    </tr>
    <tr>
      <td align="center">🗺️ <strong>Mapping</strong></td>
      <td>AutoMapper</td>
      <td><code>16.2</code></td>
      <td>Object-to-object mapping</td>
    </tr>
    <tr>
      <td align="center">⏰ <strong>Background Jobs</strong></td>
      <td>Hangfire</td>
      <td><code>1.8</code></td>
      <td>Scheduled & fire-and-forget jobs</td>
    </tr>
    <tr>
      <td align="center">📧 <strong>Email</strong></td>
      <td>MailKit</td>
      <td><code>4.17</code></td>
      <td>SMTP email delivery</td>
    </tr>
    <tr>
      <td align="center">🔑 <strong>Authentication</strong></td>
      <td>JWT Bearer + Google OAuth</td>
      <td><code>10.0</code></td>
      <td>Token-based auth & social login</td>
    </tr>
    <tr>
      <td align="center">👤 <strong>Identity</strong></td>
      <td>ASP.NET Core Identity</td>
      <td><code>10.0</code></td>
      <td>User management & role system</td>
    </tr>
    <tr>
      <td align="center">🗃️ <strong>Database</strong></td>
      <td>SQL Server</td>
      <td><code>2022</code></td>
      <td>Primary relational database</td>
    </tr>
    <tr>
      <td align="center">📊 <strong>Logging</strong></td>
      <td>Serilog</td>
      <td><code>10.0</code></td>
      <td>Structured logging (Console, File)</td>
    </tr>
    <tr>
      <td align="center">📘 <strong>API Docs</strong></td>
      <td>Scalar (OpenAPI)</td>
      <td><code>2.17</code></td>
      <td>Interactive API documentation</td>
    </tr>
    <tr>
      <td align="center">🌍 <strong>User Agent</strong></td>
      <td>UAParser</td>
      <td><code>3.1</code></td>
      <td>Client device detection</td>
    </tr>
    <tr>
      <td align="center">🐳 <strong>Containerization</strong></td>
      <td>Docker & Docker Compose</td>
      <td>—</td>
      <td>One-command deployment</td>
    </tr>
    <tr>
      <td align="center">🧪 <strong>Testing</strong></td>
      <td>xUnit / NUnit (Unit Tests)</td>
      <td>—</td>
      <td>Automated test suite</td>
    </tr>
  </tbody>
</table>

---

## 📂 Project Structure

```
ClinicOS/
├── 📁 src/
│   ├── 📁 ClinicOS.Domain/              # 🏛️ Domain Layer (innermost)
│   │   ├── Common/                       #   Base entities, domain events, Result<T>
│   │   ├── Constants/                    #   Domain constants
│   │   ├── Entities/                     #   Aggregate roots & entities
│   │   │   ├── Appointments/             #     Appointment aggregate
│   │   │   ├── AuditLogs/                #     Audit trail entity
│   │   │   ├── Doctors/                  #     Doctor aggregate
│   │   │   ├── Invitation/               #     Staff invitation entity
│   │   │   ├── MedicalRecords/           #     EMR entity
│   │   │   ├── OtpVerification/          #     OTP verification entity
│   │   │   ├── Patients/                 #     Patient aggregate
│   │   │   ├── Prescriptions/            #     E-Prescribing entity
│   │   │   ├── Receptionists/            #     Receptionist aggregate
│   │   │   └── Specializations/          #     Medical specialization entity
│   │   ├── Enums/                        #   Domain enumerations
│   │   └── Errors/                       #   Domain error definitions
│   │
│   ├── 📁 ClinicOS.Application/          # 📦 Application Layer
│   │   ├── Common/
│   │   │   ├── Abstractions/             #   Interfaces (persistence, identity, etc.)
│   │   │   ├── Behaviors/                #   MediatR pipeline behaviors (7 behaviors)
│   │   │   ├── Constants/                #   Application constants
│   │   │   ├── Errors/                   #   Application-level errors
│   │   │   ├── Helpers/                  #   Utility helpers
│   │   │   ├── Pagination/               #   Paginated query support
│   │   │   └── Validation/               #   Validation base classes
│   │   ├── Features/
│   │   │   ├── Accounts/                 #   Account management (CQRS)
│   │   │   ├── Doctors/                  #   Doctor management (CQRS)
│   │   │   ├── Otps/                     #   OTP operations (CQRS)
│   │   │   ├── Patients/                 #   Patient management (CQRS)
│   │   │   ├── Receptionists/            #   Receptionist management (CQRS)
│   │   │   └── Specializations/          #   Specialization management (CQRS)
│   │   └── DependencyInjection.cs        #   Application layer DI registration
│   │
│   ├── 📁 ClinicOS.Infrastructure/       # 🔧 Infrastructure Layer
│   │   ├── BackgroundJobs/               #   Outbox message processor
│   │   ├── Core/                         #   Core infrastructure services
│   │   ├── DependencyInjection/          #   Infrastructure DI registration
│   │   ├── External/                     #   External service integrations
│   │   │   ├── Cache/                    #     Caching (Redis / In-Memory)
│   │   │   ├── Client/                   #     HTTP client services
│   │   │   ├── Email/                    #     MailKit email + HTML templates
│   │   │   ├── FileStorage/              #     File upload & storage
│   │   │   ├── Jobs/                     #     Hangfire job scheduling
│   │   │   └── Routing/                  #     URL routing helpers
│   │   ├── Identity/                     #   ASP.NET Core Identity implementation
│   │   ├── Migrations/                   #   EF Core database migrations
│   │   ├── Notifications/                #   Notification services
│   │   ├── Options/                      #   Strongly-typed configuration (IOptions)
│   │   └── Persistence/                  #   DbContext & repository implementations
│   │
│   └── 📁 ClinicOS.Api/                  # 🌐 API Layer (outermost)
│       ├── Controllers/                  #   REST API controllers
│       ├── Contracts/                    #   Request/Response DTOs
│       ├── Extensions/                   #   Service registration extensions
│       ├── Middlewares/                  #   Global exception & correlation ID
│       ├── Program.cs                    #   Application entry point
│       ├── Dockerfile                    #   Multi-stage Docker build
│       └── appsettings.json              #   Application configuration
│
├── 📁 ClinicOS.UnitTests/                # 🧪 Unit Test Project
│   ├── Common/                           #   Test utilities & fixtures
│   └── Features/                         #   Feature-specific test suites
│
├── 🐳 docker-compose.yml                 #   Docker Compose orchestration
├── 🐳 docker-compose.override.yml        #   Development overrides
└── 📄 ClinicOS.slnx                      #   Solution file
```

---

## 🚀 Getting Started

### 📋 Prerequisites

| Requirement | Minimum Version |
|:------------|:----------------|
| [.NET SDK](https://dotnet.microsoft.com/download) | `10.0` or later |
| [SQL Server](https://www.microsoft.com/sql-server) | `2019+` (or use Docker) |
| [Docker](https://www.docker.com/) *(optional)* | `20.10+` |
| [Docker Compose](https://docs.docker.com/compose/) *(optional)* | `2.0+` |

---

### 🐳 Option 1: Docker Compose (Recommended)

The fastest way to get ClinicOS running — one command brings up the API and SQL Server:

```bash
# 1. Clone the repository
git clone https://github.com/your-username/ClinicOS.git
cd ClinicOS

# 2. Launch all services
docker-compose up -d

# 3. Access the API
#    🌐 API:       http://localhost:8080
#    📘 Scalar UI: http://localhost:8080/scalar
```

> **💡 Tip:** The database will be created and migrated automatically on first startup.

---

### 💻 Option 2: Local Development

```bash
# 1. Clone the repository
git clone https://github.com/your-username/ClinicOS.git
cd ClinicOS

# 2. Update the connection string in appsettings.json
#    Located at: src/ClinicOS.Api/appsettings.json

# 3. Apply database migrations
dotnet ef database update \
  --project src/ClinicOS.Infrastructure \
  --startup-project src/ClinicOS.Api

# 4. Run the application
dotnet run --project src/ClinicOS.Api

# 5. Access the API
#    🌐 API:       http://localhost:8080
#    📘 Scalar UI: http://localhost:8080/scalar
```

---

### ⚙️ Configuration

Key configuration sections in `appsettings.json`:

```jsonc
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=ClinicOSDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "your-secret-key",
    "Issuer": "ClinicOS",
    "Audience": "ClinicOS-Clients",
    "ExpirationInMinutes": 60
  },
  "GoogleAuth": {
    "ClientId": "your-google-client-id"
  },
  "EmailSettings": {
    "SmtpServer": "smtp.example.com",
    "Port": 587,
    "SenderEmail": "noreply@yourclinic.com"
  },
  "HangfireSettings": {
    "DashboardPath": "/hangfire"
  }
}
```

> **🔐 Security:** Use [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) or environment variables for sensitive values in development. Never commit secrets to source control.

---

## 🗺️ Roadmap

ClinicOS is under active development. Here's what's coming next:

| Phase | Module | Description | Status |
|:------|:-------|:------------|:------:|
| **Phase 1** | 🔐 Identity & Auth | Multi-strategy login, roles, permissions, JWT | ✅ Done |
| **Phase 1** | 👨‍⚕️ Staff Management | Doctors, receptionists, invitations, profiles | ✅ Done |
| **Phase 1** | 🧑‍🤝‍🧑 Patient Management | Dual-path registration, self-service portal | ✅ Done |
| **Phase 1** | 🩺 Specializations | Specialty CRUD, doctor schedules, caching | ✅ Done |
| **Phase 2** | 📅 Appointments | Online booking, waiting lists, reminders | 🔜 Next |
| **Phase 2** | 📋 Medical Records | Electronic Medical Records (EMR) | 🔜 Planned |
| **Phase 2** | 💊 Prescriptions | E-Prescribing & medication management | 🔜 Planned |
| **Phase 3** | 💰 Billing & Payments | Invoicing, online payments, financial reports | 📋 Planned |
| **Phase 3** | 🔍 Audit Logs | Full system activity tracking & security audit | 📋 Planned |
| **Phase 3** | 📊 Analytics Dashboard | Clinic KPIs, reports, and data visualization | 📋 Planned |
| **Phase 4** | 📱 Notifications | SMS, push notifications, real-time alerts | 📋 Future |
| **Phase 4** | 🌐 Multi-language | Full i18n support (Arabic, English, etc.) | 📋 Future |

---

## 🧪 Testing

ClinicOS includes a dedicated unit test project with organized test suites:

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity detailed

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

---

## 🤝 Contributing

Contributions are welcome! Whether it's bug fixes, new features, or documentation improvements — every contribution matters.

### How to Contribute

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **Commit** your changes (`git commit -m 'feat: add amazing feature'`)
4. **Push** to the branch (`git push origin feature/amazing-feature`)
5. **Open** a Pull Request

### Contribution Guidelines

- Follow the existing **Clean Architecture** layer boundaries
- Write **unit tests** for new features
- Use **CQRS** (Commands & Queries via MediatR) for all new operations
- Apply **FluentValidation** for all request validation
- Follow [Conventional Commits](https://www.conventionalcommits.org/) for commit messages

---

## 📄 License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) by Robert C. Martin
- [MediatR](https://github.com/jbogard/MediatR) by Jimmy Bogard
- [FluentValidation](https://docs.fluentvalidation.net/) by Jeremy Skinner
- [Hangfire](https://www.hangfire.io/) for reliable background processing
- [Serilog](https://serilog.net/) for structured logging excellence

---

<p align="center">
  <strong>Built with ❤️ and ☕ for the healthcare community</strong>
</p>

<p align="center">
  <sub>
    ClinicOS — Empowering clinics with enterprise-grade software, one module at a time.<br/>
    نظام ClinicOS — تمكين العيادات ببرمجيات بمعايير المؤسسات الكبرى، وحدة تلو الأخرى.
  </sub>
</p>

<p align="center">
  <a href="#-clinicos">⬆️ Back to Top</a>
</p>
