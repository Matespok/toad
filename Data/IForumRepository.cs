using toad.DTOs;

namespace toad.Repositories;

public interface IForumRepository
{
    // ==================
    // Uživatelé
    // ==================
    Task AddUserAsync(string username, string password);
    Task<int?> AuthenticateUserAsync(string username, string password);
    Task<bool> IsRegisteredAsync(string username);
    Task<UserDTO?> GetUserInfoAsync(int id);

    // ==================
    // Příspěvky (Posts)
    // ==================
    Task AddPostAsync(int userId, string topic, string content);
    Task<List<PostDTO>> GetAllPostsAsync();
    Task<PostDTO?> GetPostByIdAsync(int postId);
    Task<List<PostDTO>> GetPostsByUserIdAsync(int userId);

    // ==================
    // Komentáře (Comments)
    // ==================
    Task AddCommentAsync(int userId, int postId, int? parentId, string content);
    Task<List<CommentDTO>> GetCommentsForPostAsync(int postId);
    Task<List<CommentDTO>> GetCommentsByUserIdAsync(int userId);
}

