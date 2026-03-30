# TOAD (TOAD Organizes All Discussions)

This is the first alpha version of a simple web forum built with C# and ASP.NET Core Razor Pages, using a PostgreSQL database.

The project is currently in early development. The backend and database models are functional, but the frontend, design, and overall layout are still being unified and adjusted.

## Technologies Used

* C# / .NET
* ASP.NET Core Razor Pages
* ADO.NET (Npgsql)
* BCrypt.Net
* PostgreSQL

## Running Locally

To run the project on your local machine, follow these steps:

1. Clone this repository.
2. In the project's root directory, rename the `appsettings.example.json` file to `appsettings.json`.
3. Update the connection string in the `appsettings.json` file to match your local PostgreSQL database credentials.
4. Run the application:
   ```bash
   dotnet run
   ```
## To-Do / Roadmap
- [ ] Docker :)
- [ ] Db connection close or just begin one, not 100
- [ ] Wrap comments and threads
- [ ] Verify if username already exists during registration
- [ ] Implement replies to comments
- [ ] Display user profiles, their threads, and replies
- [ ] Improve navigation (`<aside>` panel)
- [ ] Limit maximum input length and implement rate limiting

