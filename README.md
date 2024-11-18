# LibraryVault

## Overview
LibraryVault is a comprehensive library management system designed to streamline book and user management for libraries.

## Project Structure
- **Domain Layer**: Contains core entities and value objects
- **Application Layer**: Implements business logic and service interfaces
- **Infrastructure Layer**: Handles data persistence and external integrations

## Key Components

### Entities
- **Book**: Represents library books with properties like Title, Author, ISBN, and Publication Year
- **User**: Manages user accounts with roles and authentication

### Services
- **BookService**: Provides CRUD operations for books
- **UserService**: Handles user management and authentication

### Key Features
- Book management (create, update, delete, search)
- User registration and authentication
- Secure password hashing
- Role-based access control

## Technologies
- .NET Core
- Entity Framework Core
- BCrypt for password hashing
- xUnit for unit testing
- Moq for mocking

## Getting Started

### Prerequisites
- .NET 6.0 or later
- Visual Studio 2022 or VS Code

### Installation
1. Clone the repository
2. Restore NuGet packages
3. Configure database connection
4. Run migrations
5. Build and run the application

## Testing
Run unit tests using:
```
dotnet test
```

## Authentication
- Passwords are securely hashed using BCrypt
- User roles include Admin and standard user

## Contributing
1. Fork the repository
2. Create a feature branch
3. Commit changes
4. Push and create a pull request
