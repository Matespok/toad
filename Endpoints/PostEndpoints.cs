using System.Security.Claims;
using toad.Repositories;
namespace toad.Endpoints;

public static class PostEndpoints
{
    public static void MapPostEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/posts")
                             .WithTags("Posts");

        // GET all posts
        group.MapGet("/", async (IForumRepository repo) =>
        {
            var posts = await repo.GetAllPostsAsync();
            return posts != null ? Results.Ok(posts) : Results.NoContent();
        })
        .WithSummary("Get all posts")
        .WithDescription("Retrieves a list of all available posts.");

        // GET post by ID
        group.MapGet("/{id}", async (int id, IForumRepository repo) =>
        {
            var post = await repo.GetPostByIdAsync(id); // await je nutný
            return post != null ? Results.Ok(post) : Results.NotFound();
        })
        .WithSummary("Get post by ID")
        .WithDescription("Retrieves a specific post by its unique identifier.");

        // POST add post
        

        group.MapPost("/", async (ClaimsPrincipal user, string topic, string content, IForumRepository repo) =>
            {
                try
                {
                    
                    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                                      ?? user.FindFirst("sub")?.Value;

                    if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int authenticatedUserId))
                    {
                        return Results.Unauthorized();
                    }
                    
                    await repo.AddPostAsync(authenticatedUserId, topic, content); 
                    return Results.Created($"/api/posts/{authenticatedUserId}", null);
                }
                catch (Exception e)
                {
                    return Results.BadRequest(e.Message);
                }
            }).RequireAuthorization()
            .WithSummary("Create a new post")
            .WithDescription("Adds a new post to the system. Requires valid jwt token");

        // GET posts by user ID
        group.MapGet("/user/{id}", async (int id, IForumRepository repo) =>
        {
            var posts = await repo.GetPostsByUserIdAsync(id);
            return posts != null ? Results.Ok(posts) : Results.NotFound();
        })
        .WithSummary("Get posts by user ID")
        .WithDescription("Retrieves all posts authored by a specific user.");
    }
}