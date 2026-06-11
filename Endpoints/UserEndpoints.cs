using toad.Repositories;
using toad.Service;

namespace toad.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/user").WithTags("Users");

        // API POST - Add user
        group.MapPost("/", async (string uname, string password, IForumRepository repo) =>
            {
                await repo.AddUserAsync(uname, password);
                return Results.Ok();
            })
            .WithSummary("Register a new user")
            .WithDescription("Creates a new user account in the system.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        // API GET - Authenticate user

        group.MapGet("/authenticate", async (string uname, string pass, IForumRepository repo) =>
            {
                var uId = await repo.AuthenticateUserAsync(uname, pass);
                if (uId is not null)
                {
                    var jwtService = new JwtService();
                    var token = jwtService.GenerateToken(uId.Value, uname);
                    return Results.Ok(new { token = token, userId = uId.Value, username = uname });
                }
                return Results.Unauthorized();
            })
            .WithSummary("Authenticate user")
            .WithDescription("Validates username and password and returns jwt token, user id and username.");

        // API GET - Taken username
        group.MapGet("/exists", async (string uname, IForumRepository repo) =>
            {
                var registered = await repo.IsRegisteredAsync(uname);
                return Results.Ok(registered);
            })
            .WithSummary("Check username availability")
            .WithDescription("Returns true if the username is already taken, false otherwise.");

        // API GET - Get user info
        group.MapGet("/{id}", async (int id, IForumRepository repo) =>
            {
                var uInfo = await repo.GetUserInfoAsync(id);
                return uInfo != null ? Results.Ok(uInfo) : Results.NotFound();
            })
            .WithSummary("Get user profile")
            .WithDescription("Retrieves detailed information about a user by their ID.");
    }
}