namespace toad.Endpoints;

using System.ComponentModel.DataAnnotations;
using toad.DTOs;
using toad.Repositories;
using toad.Service;

public record LoginDto(string Username, string Password);

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/user").WithTags("Users");

        // API POST - Add user
        group
            .MapPost(
                "/",
                async (string uname, string password, IForumRepository repo) =>
                {
                    await repo.AddUserAsync(uname, password);
                    return Results.Ok();
                }
            )
            .WithSummary("Register a new user")
            .WithDescription("Creates a new user account in the system.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        // API POST - Authenticate user

        group
            .MapPost(
                "/authenticate",
                async (LoginDTO model, IForumRepository repo) =>
                {
                    if (model.Username is null || model.Password is null)
                    {
                        return Results.BadRequest("Username or password is required.");
                    }
                    var uId = await repo.AuthenticateUserAsync(model.Username, model.Password);

                    if (uId is not null)
                    {
                        var jwtService = new JwtService();
                        var token = jwtService.GenerateToken(uId.Value, model.Username);
                        return Results.Ok(
                            new
                            {
                                token,
                                userId = uId.Value,
                                username = model.Username,
                            }
                        );
                    }
                    return Results.Unauthorized();
                }
            )
            .WithSummary("Authenticate user")
            .WithDescription(
                "Validates username and password via request body " + "and returns jwt token"
            );

        // API GET - Taken username
        group
            .MapGet(
                "/exists",
                async (string uname, IForumRepository repo) =>
                {
                    var registered = await repo.IsRegisteredAsync(uname);
                    return Results.Ok(registered);
                }
            )
            .WithSummary("Check username availability")
            .WithDescription("Returns true if the username is already taken, false otherwise.");

        // API GET - Get user info
        group
            .MapGet(
                "/{id}",
                async (int id, IForumRepository repo) =>
                {
                    var uInfo = await repo.GetUserInfoAsync(id);
                    return uInfo != null ? Results.Ok(uInfo) : Results.NotFound();
                }
            )
            .WithSummary("Get user profile")
            .WithDescription("Retrieves detailed information about a user by their ID.");
    }
}
