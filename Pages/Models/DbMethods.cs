using BCrypt.Net;
using hopefullyAWebForum.Pages.Models;
using Npgsql;

namespace hopefullyAWebForum.Pages.Models;

public class DbMethods
{
    private static string? _connectionString;

    public static void SetConnectionString(string connStr)
    {
        _connectionString = connStr;
    }

    /*
     * =================
     * Initialize DB
     * =================
*/

    public async Task InitializeDbAsync()
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        //Table Users
        await using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText =
                @"
                CREATE TABLE IF NOT EXISTS ""Users"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    ""Username"" TEXT UNIQUE NOT NULL,
                    ""Password"" TEXT NOT NULL,
                    ""JoinDate"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                    );";

            await cmd.ExecuteNonQueryAsync();

            //Table posts
            cmd.CommandText =
                @"
            CREATE TABLE IF NOT EXISTS ""Posts"" (
                ""PostId"" SERIAL PRIMARY KEY,
                ""UserId"" INTEGER NOT NULL,
                ""Topic"" TEXT NOT NULL,
                ""Content"" TEXT NOT NULL,
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                CONSTRAINT fk_user FOREIGN KEY(""UserId"") REFERENCES ""Users""(""Id"") ON DELETE CASCADE
            );";
            await cmd.ExecuteNonQueryAsync();

            //Table comments
            cmd.CommandText =
                @"
            CREATE TABLE IF NOT EXISTS ""Comments"" (
                ""CommentId"" SERIAL PRIMARY KEY,
                ""UserId"" INTEGER NOT NULL,
                ""PostId"" INTEGER NOT NULL,
                ""ParentCommentId"" INTEGER,
                ""Content"" TEXT NOT NULL,
                ""CommentedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                CONSTRAINT fk_user 
                FOREIGN KEY(""UserId"") 
                REFERENCES ""Users""(""Id"") 
                ON DELETE CASCADE,
                
                CONSTRAINT fk_post 
                FOREIGN KEY (""PostId"") 
                REFERENCES ""Posts""(""PostId"") 
                ON DELETE CASCADE,
                
                CONSTRAINT fk_parent
                FOREIGN KEY (""ParentCommentId"")
                REFERENCES ""Comments""(""CommentId"")
                ON DELETE CASCADE
                );
             ";
            await cmd.ExecuteNonQueryAsync();
        }
    }

    /*
     * ==================
     * User registration
     * ==================
     */

    public async Task AddUserAsync(string username, string password)
    {
        string passHash = BCrypt.Net.BCrypt.HashPassword(password);
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        await using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText =
                @"
          INSERT INTO ""Users"" (""Username"", ""Password"")
          VALUES (@uname, @pass);
          ";
            cmd.Parameters.AddWithValue("@uname", username);
            cmd.Parameters.AddWithValue("@pass", passHash);

            await cmd.ExecuteNonQueryAsync();
        }
    }

    /*
     * =============
     * USER LOGIN
     * =============
    *
    */

    public async Task<int?> AuthenticateUserAsync(string username, string password)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        await using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText =
                @"
            SELECT ""Id"", ""Password"" 
            FROM ""Users"" 
            WHERE ""Username"" = @uname;";
            cmd.Parameters.AddWithValue("@uname", username);

            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    int id = reader.GetInt32(0);
                    string storedHash = reader.GetString(1);

                    if (BCrypt.Net.BCrypt.Verify(password, storedHash))
                    {
                        return id;
                    }
                }
            }
            return null;
        }
    }

    /*
     * =============
     * Post creation
     * =============
    */

    public async Task AddPostAsync(int userId, string topic, string content)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        await using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText =
                @"
          INSERT INTO ""Posts"" (""UserId"", ""Topic"", ""Content"")
          VALUES (@userId, @topic, @content);
          ";
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@topic", topic);
            cmd.Parameters.AddWithValue("@content", content);
            await cmd.ExecuteNonQueryAsync();
        }
    }

    /*
     * ===========
     * post read
     * ===========
     * */

    public async Task<List<Post>> ReadPostAsync()
    {
        List<Post> postList = new();
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        await using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText =
                @"
            SELECT p.""PostId"", u.""Username"", p.""UserId"", p.""Topic"", p.""Content"", p.""CreatedAt""
                    FROM ""Posts"" p, ""Users"" u
                    WHERE p.""UserId"" = u.""Id""
                    ORDER BY p.""CreatedAt"" DESC;
            ";
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var post1 = new Post
                    {
                        PostId = reader.GetInt32(0),
                        AuthorName = reader.GetString(1),
                        UserId = reader.GetInt32(2),
                        Topic = reader.GetString(3),
                        Content = reader.GetString(4),
                        CreatedAt = reader.GetDateTime(5),
                    };
                    postList.Add(post1);
                }
            }
            return postList;
        }
    }

    public async Task<Post?> ShowPostAsync(int postId)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        await using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText =
                @"
            SELECT p.""PostId"", u.""Username"", p.""UserId"", p.""Topic"", p.""Content"", p.""CreatedAt""
            FROM ""Posts"" p
            JOIN ""Users"" u ON p.""UserId"" = u.""Id""
            WHERE p.""PostId"" = @postId";
            cmd.Parameters.AddWithValue("@postId", postId);

            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new Post
                    {
                        PostId = reader.GetInt32(0),
                        AuthorName = reader.GetString(1),
                        UserId = reader.GetInt32(2),
                        Topic = reader.GetString(3),
                        Content = reader.GetString(4),
                        CreatedAt = reader.GetDateTime(5),
                    };
                }
            }
        }
        return null;
    }

    public async Task AddCommentAsync(int userId, int postId, int? parentId, string content)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        await using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText =
                @"
            INSERT INTO ""Comments"" (""UserId"", ""PostId"", ""ParentCommentId"", ""Content"")
            VALUES (@userId, @postId, @parentId, @content);
          ";

            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@postId", postId);
            cmd.Parameters.AddWithValue("@parentId", (object)parentId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@content", content);

            await cmd.ExecuteNonQueryAsync();
        }
    }

    public async Task<List<Comment>> ShowCommentsAsync(int postId)
    {
        List<Comment> allComments = new();

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        await using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText =
                @"
            SELECT u.""Username"", c.""ParentCommentId"", c.""CommentedAt"", c.""Content""
            FROM ""Comments"" c
            JOIN ""Users"" u ON c.""UserId"" = u.""Id""
            WHERE c.""PostId"" = @postId
            ORDER BY ""CommentedAt"" DESC;
            ";

            cmd.Parameters.AddWithValue("@postId", postId);

            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var comment1 = new Comment
                    {
                        AuthorName = reader.GetString(0),
                        ParentCommentId = reader.IsDBNull(1) ? 0 : reader.GetInt32(1), // NULL OR NOT NULL
                        CommentedAt = reader.GetDateTime(2),
                        Content = reader.GetString(3),
                    };
                    allComments.Add(comment1);
                }
            }
            return allComments;
        }
    }

    public async Task<bool> IsRegistered(string uname)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        await using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText =
                @"
        SELECT ""Username""
        FROM ""Users""
        WHERE ""Username"" = @uname
        ";
            cmd.Parameters.AddWithValue("@uname", uname);
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return true;
                }
            }
            return false;
        }
    }

    /*
     *  first i need the user and the info about him
     * */

    public async Task<User?> GetUserInfoAsync(int id)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        await using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText =
                @"
                  SELECT ""Username"", ""JoinDate""
                  FROM ""Users""
                  WHERE ""Id"" = @id;
                  ";
            cmd.Parameters.AddWithValue("@id", id);

            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new User
                    {
                        Username = reader.GetString(0),
                        JoinDate = reader.GetDateTime(1),
                    };
                }
            }
        }
        return null;
    }

    public async Task<List<Post>> GetUserPostsAsync(int id)
    {
        List<Post> postList = new();
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        await using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText =
                @"
            SELECT p.""PostId"", u.""Username"", p.""Topic"", p.""Content"", p.""CreatedAt""
                    FROM ""Posts"" p, ""Users"" u
                    WHERE p.""UserId"" = u.""Id"" AND p.""UserId"" = @userId
                    ORDER BY p.""CreatedAt"" DESC;
            ";
            cmd.Parameters.AddWithValue("@userId", id);
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var post1 = new Post
                    {
                        PostId = reader.GetInt32(0),
                        AuthorName = reader.GetString(1),
                        Topic = reader.GetString(2),
                        Content = reader.GetString(3),
                        CreatedAt = reader.GetDateTime(4),
                    };
                    postList.Add(post1);
                }
            }
            return postList;
        }
    }

    public async Task<List<Comment>> GetUserCommentsAsync(int id)
    {
        List<Comment> allComments = new();

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        await using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText =
                @"
            SELECT u.""Username"", c.""PostId"", c.""ParentCommentId"", c.""CommentedAt"", c.""Content""
            FROM ""Comments"" c
            JOIN ""Users"" u ON c.""UserId"" = u.""Id""
            WHERE c.""UserId"" = @userId
            ORDER BY ""CommentedAt"" DESC;
            ";

            cmd.Parameters.AddWithValue("@userId", id);

            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var comment1 = new Comment
                    {
                        AuthorName = reader.GetString(0),
                        PostId = reader.GetInt32(1),
                        ParentCommentId = reader.IsDBNull(2) ? 0 : reader.GetInt32(1), // NULL OR NOT NULL
                        CommentedAt = reader.GetDateTime(3),
                        Content = reader.GetString(4),
                    };
                    allComments.Add(comment1);
                }
            }
            return allComments;
        }
    }
}
