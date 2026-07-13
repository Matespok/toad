# TOAD (TOAD Organizes All Discussions)

This is the first alpha version of a simple web forum built with C# and ASP.NET Core Razor Pages, using a PostgreSQL database.

The project is currently in early development. The backend and database models are functional, but the frontend, design, and overall layout are still being unified and adjusted. Also Iˇm aware of most security flaws this project has.

## Technologies Used

* C# / .NET
* ASP.NET Core Razor Pages
* EF core (Npgsql)
* BCrypt.Net
* PostgreSQL
* Docker
## Running Locally
Follow these steps to get the project up and running on your local machine:

# TO BE UPDATED!!

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

3. Start Docker
   ```bash 
      docker-compose up --build
      ```

## To-Do / Roadmap
- [X] Docker :)
- [X] Async db from sync, connections opened asynchronously
- [X] Wrap comments and threads
- [X] Verify if username already exists during registration,
- [X] after registration login or tell user to do so.
- [X] Implement replies to comments
- [X] Display user profiles, their threads, and replies
- [ ] Improve navigation (`<aside>` panel)
- [ ] Limit maximum input length and implement rate limiting
- [X] Rewrote methods to EF CORE
- [X] Dependency injection
