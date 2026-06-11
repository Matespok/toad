using Microsoft.EntityFrameworkCore;
using toad.Data;
using toad.Data.Entities;
using toad.DTOs;

namespace toad.Repositories;

public class ForumRepository : IForumRepository
{
    private readonly ApplicationDbContext _context;

    public ForumRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /*
     * ==================
     * Uživatelé
     * ==================
     */
    public async Task AddUserAsync(string username, string password)
    {
        var passHash = BCrypt.Net.BCrypt.HashPassword(password);

        var newUser = new UserEntity { Username = username, Password = passHash };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
    }

    public async Task<int?> AuthenticateUserAsync(string username, string password)
    {
        var user = await _context
            .Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            return user.Id;
        }

        return null;
    }

    public async Task<bool> IsRegisteredAsync(string username)
    {
        return await _context.Users.AsNoTracking().AnyAsync(u => u.Username == username);
    }

    public async Task<UserDTO?> GetUserInfoAsync(int id)
    {
        return await _context
            .Users.AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UserDTO { Username = u.Username, JoinDate = u.JoinDate })
            .FirstOrDefaultAsync();
    }

    /*
     * ==================
     * Příspěvky (Posts)
     * ==================
     */
    public async Task AddPostAsync(int userId, string topic, string content)
    {
        var post = new PostEntity
        {
            UserId = userId,
            Topic = topic,
            Content = content,
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
    }

    public async Task<List<PostDTO>> GetAllPostsAsync()
    {
        return await _context
            .Posts.AsNoTracking()
            .Include(p => p.User) // Propojení s autorem, abychom měli Username
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PostDTO
            {
                PostId = p.PostId,
                UserId = p.UserId,
                AuthorName = p.User.Username,
                Topic = p.Topic,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
            })
            .ToListAsync();
    }

    public async Task<PostDTO?> GetPostByIdAsync(int postId)
    {
        return await _context
            .Posts.AsNoTracking()
            .Include(p => p.User)
            .Where(p => p.PostId == postId)
            .Select(p => new PostDTO
            {
                PostId = p.PostId,
                AuthorName = p.User.Username,
                UserId = p.UserId,
                Topic = p.Topic,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<PostDTO>> GetPostsByUserIdAsync(int userId)
    {
        return await _context
            .Posts.AsNoTracking()
            .Include(p => p.User)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PostDTO
            {
                PostId = p.PostId,
                AuthorName = p.User.Username,
                Topic = p.Topic,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
            })
            .ToListAsync();
    }

    /*
     * ==================
     * Komentáře
     * ==================
     */
    public async Task AddCommentAsync(int userId, int postId, int? parentId, string content)
    {
        var comment = new CommentEntity
        {
            UserId = userId,
            PostId = postId,
            ParentCommentId = parentId,
            Content = content,
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
    }

    public async Task<List<CommentDTO>> GetCommentsForPostAsync(int postId)
    {
        return await _context
            .Comments.AsNoTracking()
            .Include(c => c.User)
            .Where(c => c.PostId == postId)
            .OrderByDescending(c => c.CommentedAt)
            .Select(c => new CommentDTO
            {
                Id = c.CommentId, 
                AuthorName = c.User.Username,
                UserId =  c.UserId,
                PostId = c.PostId,
                ParentCommentId = c.ParentCommentId,
                Content = c.Content,
                CommentedAt = c.CommentedAt,
            })
            .ToListAsync();
    }

    public async Task<List<CommentDTO>> GetCommentsByUserIdAsync(int userId)
    {
        return await _context
            .Comments.AsNoTracking()
            .Include(c => c.User)
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CommentedAt)
            .Select(c => new CommentDTO
            {
                AuthorName = c.User.Username,
                PostId = c.PostId,
                ParentCommentId = c.ParentCommentId ?? 0,
                Content = c.Content,
                CommentedAt = c.CommentedAt,
            })
            .ToListAsync();
    }
}
