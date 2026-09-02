using System.Security.Claims;
using toad.Repositories;

namespace toad.Endpoints;

public static class CommentEndpoints
{
    public static void MapCommentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/comments").WithTags("Comments");

        // API POST - Add comments
        group
            .MapPost(
                "/",
                async (
                    ClaimsPrincipal user,
                    int postId,
                    int? parentId,
                    string content,
                    IForumRepository repo
                ) =>
                {
                    try
                    {
                        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                        if (
                            string.IsNullOrEmpty(userIdClaim)
                            || !int.TryParse(userIdClaim, out int authenticatedUserId)
                        )
                        {
                            return Results.Unauthorized();
                        }

                        // 2. Použít bezpečné, ověřené ID a přidat chybějící AWAIT!
                        await repo.AddCommentAsync(authenticatedUserId, postId, parentId, content);

                        return Results.Ok();
                    }
                    catch (Exception e)
                    {
                        return Results.Conflict(e.Message);
                    }
                }
            )
            .RequireAuthorization()
            .WithSummary("Creates a new comment")
            .WithDescription(
                "Adds a new comment to a specific post. Requires postId and content. Requires valid JWT token."
            );

        // API GET - Get comments for post
        group
            .MapGet(
                "/{id}",
                async (int id, IForumRepository repo) =>
                {
                    try
                    {
                        var comments = await repo.GetCommentsForPostAsync(id);
                        return comments is not null ? Results.Ok(comments) : Results.NoContent();
                    }
                    catch (Exception e)
                    {
                        return Results.Conflict(e.Message);
                    }
                }
            )
            .WithSummary("Retrieves comments for a post")
            .WithDescription(
                "Fetches a list of all comments associated with " + "the specified post ID."
            );

        // API GET - Comments for user
        group
            .MapGet(
                "/users/{id}",
                async (int id, IForumRepository repo) =>
                {
                    try
                    {
                        var comments = await repo.GetCommentsByUserIdAsync(id);
                        return comments is not null ? Results.Ok(comments) : Results.NoContent();
                    }
                    catch (Exception e)
                    {
                        return Results.Conflict(e.Message);
                    }
                }
            )
            .WithSummary("Retrieves comments by user")
            .WithDescription("Fetches all comments authored by a specific user.");
    }
}
