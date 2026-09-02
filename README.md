# TOAD (TOAD Organizes All Discussions)

This is the first alpha version of a simple web forum built with C# and ASP.NET Core Razor Pages, using a PostgreSQL database.

The project is currently in early development. The backend and database models are functional, but the frontend, design, and overall layout are still being unified and adjusted. Also Iˇm aware of most security flaws this project has.

## Technologies Used

- C# / .NET
- ASP.NET Core Razor Pages
- EF core (Npgsql)
- BCrypt.Net
- PostgreSQL
- Docker

## Running Locally

# TOAD (TOAD Organizes All Discussions)

This is the first alpha version of a simple web forum built with C# and ASP.NET Core Razor Pages, using a PostgreSQL database.

The project is currently in early development. The backend and database models are functional, but the frontend, design, and overall layout are still being unified and adjusted. Also Iˇm aware of most security flaws this project has.

## Technologies Used

- C# / .NET
- ASP.NET Core Razor Pages
- EF core (Npgsql)
- BCrypt.Net
- PostgreSQL
- Docker

## Running Locally

Follow these steps to get the project up and running on your local machine:

### 1. Prerequisites

Ensure you have the following installed:

- **Docker** & **Docker Compose**

### 2. Environment Setup

1. **Clone the repository:**
   ```bash
   git clone https://github.com/Matespok/toad.git
   ```

**2. Configure environment variables:**

- create `.env` file, and configure your variables
  ```bash
  DB_USER=toaduser
  DB_PASSWORD=toadpass
  DB_NAME=toaDB
  JWTKEY=toad-jwt-key-long-password
  ```

3. Start Docker
   ```bash
    docker-compose up --build
   ```

## To-Do / Roadmap

- [x] Docker :)
- [x] Async db from sync, connections opened asynchronously
- [x] Wrap comments and threads
- [x] Verify if username already exists during registration,
- [x] after registration login or tell user to do so.
- [x] Implement replies to comments
- [x] Display user profiles, their threads, and replies
- [ ] Improve navigation (`<aside>` panel)
- [ ] Limit maximum input length and implement rate limiting
- [x] Rewrote methods to EF CORE
- [x] Dependency injection
