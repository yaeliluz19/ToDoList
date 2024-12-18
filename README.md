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

### 1. **Get All Tasks**

**Route:**  
`GET /api/todolist`

**Description:**  
Retrieves all tasks from the database.

**Response Example:**
```json
[
  {
    "id": 1,
    "title": "Task 1",
    "description": "Description of Task 1",
    "completed": false
  },
  {
    "id": 2,
    "title": "Task 2",
    "description": "Description of Task 2",
    "completed": true
  }
]

“### 2\. **Add a New Task** **Route:** `POST /api/todolist` **Description:** Adds a new task to the list. **Request Example:** json Copy code `{ "title": "New Task", "description": "Description of the task" }` * * *
“### 3\. **Update Task** **Route:** `PUT /api/todolist/{id}` **Description:** Updates an existing task by its ID. **Request Example:** json Copy code `{ "title": "Updated Task", "description": "Updated description of the task", "completed": true }`”
“### 4\. **Delete Task** **Route:** `DELETE /api/todolist/{id}` **Description:** Deletes a task by its ID. * * * JWT Authentication ------------------”
“The project includes a JWT-based authentication mechanism to ensure secure access to the API. ### 1\. **User Table** The user table includes the following fields: * `id`: Unique identifier for the user. * `username`: Username. * `password`: Password (stored securely).”
“**Response Example:** json Copy code `{ "token": "your_jwt_token_here" }` The token will be used for all protected API calls. ### 3\. **Registration and Login Pages**”
“On the client-side, we have a registration page for new users to create an account, and a login page for existing users to authenticate and receive the JWT.”
“Axios Interceptor ----------------- We’ve added an **Axios Interceptor** that handles `401 Unauthorized` errors by redirecting the user to the login page in case of an authentication issue (e.g., expired token). ### Example Interceptor:”
“axios.interceptors.response.use( response => response, error => { if (error.response.status === 401) { window.location.href = '/login'; // Redirect to login page } return Promise.reject(error); } );”
“Environment Setup ----------------- * **.NET 6** (or higher) * **JWT Token** for secure authentication. * A database for storing users and tasks (SQL Server, SQLite, or any other supported database).”
“Installation & Running ---------------------- 1. **Step 1:** Run `dotnet restore` to install dependencies. 2. **Step 2:** Run `dotnet build` to build the project. 3. **Step 3:** Run `dotnet run` to start the server.”
“Issues & Support ---------------- If you have any questions or run into issues during installation or usage, feel free to reach out via GitHub Issues or send me a direct message.”
