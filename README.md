# Gym Management System

A web-based Gym Management System built with **ASP.NET Core MVC** to manage gym members, trainers, plans, sessions, memberships, categories, and health records.

## 🚀 Technologies

- C#
- ASP.NET Core MVC
- .NET 9
- Entity Framework Core
- SQL Server
- LINQ
- AutoMapper
- Repository Pattern
- Unit of Work Pattern
- Dependency Injection
- Razor Views
- Bootstrap

## 🏗️ Architecture

The project follows a layered architecture:

- **Presentation Layer** — ASP.NET Core MVC
- **Business Logic Layer (BLL)** — Business services and application logic
- **Data Access Layer (DAL)** — Entity Framework Core, repositories, and database access

### Project Layers

\Gym-Project
│
├── GymCore Project
│   └── Presentation Layer
│
├── GymManagement.BLL
│   └── Business Logic Layer
│
└── GymManagement.DAL
    └── Data Access Layer
