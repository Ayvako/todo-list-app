Collaborative Task Management System

Key Features:
- User Authentication & Security: Secure registration and login system using JWT (JSON Web Tokens) for stateless authentication.
- Collaborative Lists: Create and share to-do lists with team members. Includes a role-based access system: Owner, Editor, and Viewer.
- Task Management: Comprehensive CRUD operations for tasks, including descriptions, due dates, and status tracking (Not Started, In Progress, Completed).
- Advanced Filtering & Search: Find tasks quickly by title or filter them by creation date, due date, and current status.
- Assignment System: Assign specific tasks to users to streamline team workflow.
- Collaboration Tools: Built-in commenting system for individual tasks and tagging for better organization.
- API Documentation: Documented interactive API using Swagger/OpenAPI.

Tech Stack
Backend: C#, ASP.NET Core Web API.

Database & ORM: Entity Framework Core for database management and migrations.

Authentication: JWT Bearer Authentication.

Frontend: Razor.

Tools: Swagger UI for API testing and documentation.


Installation & Setup
- Clone the repository:

``` 
git clone https://github.com/your-username/project-alpha.git
```
- Configure Database: Update the connection string in appsettings.json.

- Run Migrations:

```
dotnet ef database update
```
- Launch the App:

```
dotnet run
```
- Access Swagger: Open https://localhost:your-port/swagger to explore the API endpoints.
