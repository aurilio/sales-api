# 🛒 Sales API

This is a RESTful API for sales management, designed with Clean Architecture principles and modern technologies such as .NET 8 and PostgreSQL.

---

## 🚀 Tecnologias e Padrões Utilizados

- **.NET 8**
- **PostgreSQL** as the relational database
- **FluentValidation** for request validations
- **AutoMapper** for object mapping
- **xUnit**, **FluentAssertions**, **Bogus**, **AutoFixture**, **NSubstitute** for unit testing
- **Serilog** for structured logging and traceability
- **Docker e Docker Compose**  for service orchestration
- **Swagger** for interactive API documentation
- Best practices including **SOLID**, **DDD**, **Clean Code**, **GitFlow** e **Semantic Commit**

---


## 🧱 Project Structure

```bash
├── src
│   ├── Application       # Handlers and commands
│   ├── Domain            # Entities, Value Objects, business rules
│   ├── ORM               # Repositories and EF Core
│   ├── Messaging         # Domain event publication
│   ├── Common            # CrossCutting: Logging, Exceptions, Validations
│   ├── WebApi            # Web API (Swagger, Middleware, Controllers)
│   └── IoC               # Dependency injection setup
├── tests
│   └── Unit              # Unit tests
├── docker-compose.yml    # Compose for local development
├── README.md             # This file

```

## ✅ Requirements

Make sure the following are installed on your machine:

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)


## ⚙️ Running Locally

### 1. Clone the repository
```bash
git clone https://github.com/aurilio/sales-api.git
cd sales-api
```

### 2. Build and run with Docker Compose

```bash
docker-compose up -d
```

### 3. Start the API manually (if not using Docker)
```bash
cd src/Ambev.DeveloperEvaluation.WebApi
dotnet run
```

## 🐳 Run via Docker (pre-built image on Docker Hub)

### Steps

**Clone the repository:**

```bash
git clone https://github.com/aurilio/sales-api.git

cd sales-api/docker

docker compose up
```

Access the application
Open your browser and navigate to:

```bash

Swagger (API)	http://localhost:8080/swagger/index.html
Frontend (UI)	http://localhost:4200 - already integrated with the API

```


## 🧪 Running Tests

To execute the unit tests::

```bash
cd tests/Ambev.DeveloperEvaluation.Unit
dotnet test

```


## 📦 Domain Event Publication

The following domain events are published and logged:

- `CreatedEvent`
- `ModifiedEvent`
- `CancelledEvent`
- `DeleteEvent`


## 📝 Commits & Versioning

- **[Semantic Commit Messages](https://www.conventionalcommits.org/en/v1.0.0/)**
- **GitFlow**.


## 📌 Notes

- Exception handling is centralized using ValidationExceptionMiddleware.
- Ready for cloud deployment with containers.
- Business rules and validations are enforced within the domain layer.

## 👨‍💻 Author

**Aurílio Mendes**  
Senior .NET Developer | Aspiring Software Architect
LinkedIn: [@aurilio](https://www.linkedin.com/in/auriliomendes/)


## 📃 License

This project is for educational purposes only.
