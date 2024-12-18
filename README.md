# ToDoList API

This project provides a RESTful API for managing to-do tasks. It includes functionalities for adding, updating, retrieving, and deleting tasks, with JWT authentication support.

### Project Requirements:

The project focuses on managing tasks through a RESTful API, supporting JWT for secure access. Each function is defined with its own route, based on the appropriate HTTP method (GET/POST/PUT/DELETE).

---

## Project Structure

- **Controllers/**: Contains the files for handling task operations.
- **Models/**: Models for tasks and users.
- **Services/**: Business logic for interacting with the database.
- **Program.cs**: Defines the API and the routes.

---

## API Endpoints

### 2. **Add a New Task**

**Route:**  
`POST /api/todolist`

**Description:**  
Adds a new task to the list.

**Request Example:**
```json
{
  "title": "New Task",
  "description": "Description of the task"
}
