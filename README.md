# TOAD (TOAD Organizes All Discussions)

This is the first alpha version of a simple web forum built with C# and ASP.NET Core Razor Pages, using a PostgreSQL database.

The project is currently in early development. The backend and database models are functional, but the frontend, design, and overall layout are still being unified and adjusted.

## Technologies Used

* C# / .NET
* ASP.NET Core Razor Pages
* ADO.NET (Npgsql)
* BCrypt.Net
* PostgreSQL
* Docker
## Running Locally
Follow these steps to get the project up and running on your local machine:

### 1. Prerequisites
Ensure you have the following installed:
* **Docker** & **Docker Compose**

### 2. Environment Setup
1. **Clone the repository:**
    ```bash
   git clone https://github.com/Matespok/toad.git
   ```
2. Configure environment variables:
- rename **.env.example** file to **.env** and put your variables into it.
  *(Note: Application connects to the database using these values.
  You can leave the defaults for testing locally.)*

3. Configure the app settings
- rename **appsettings.example.json** to **appsettings.json**

4. Start Docker
   ```bash 
      docker-compose up --build
      ```

## To-Do / Roadmap
- [X] Docker :)
- [X] Async db from sync, connections opened asynchronously
- [X] Wrap comments and threads
- [X] Verify if username already exists during registration,
- [X] after registration login or tell user to do so.
- [ ] Implement replies to comments
- [ ] ~~Display user profiles, their threads,~~ and replies
- [ ] Improve navigation (`<aside>` panel)
- [ ] Limit maximum input length and implement rate limiting
- [ ] Rewrite dbmethods to Dapper
- [ ] Dependency injection
