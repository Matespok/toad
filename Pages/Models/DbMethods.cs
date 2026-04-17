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
                    ""Password"" TEXT NOT NULL
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
    /*public static void AddUser(string username, string password)
    {
        if (_conn.State != ConnectionState.Open)
            _conn.Open();

        using (var cmd = _conn.CreateCommand())
        {
            string passHash = BCrypt.Net.BCrypt.HashPassword(password);
            cmd.CommandText =
                @"
                INSERT INTO ""Users"" (""Username"", ""Password"")
                VALUES (@uname, @pass);";
            cmd.Parameters.AddWithValue("@uname", username);
            cmd.Parameters.AddWithValue("@pass", passHash);

            cmd.ExecuteNonQuery();
        }
    }*/

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

    /*public static int? AuthenticateUser(string username, string password)
    {
        if (_conn.State != ConnectionState.Open)
            _conn.Open();

        using (var cmd = _conn.CreateCommand)
        {
            cmd.CommandText =
                @"
            SELECT ""Id"", ""Password""
            FROM ""Users""
            WHERE ""Username"" = @uname;";
            cmd.Parameters.AddWithValue("@uname", username);

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
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
    }*/

    /*
     * =============
     * Post creation
     * =============
    */
    /*public static void AddPost(int? userId, string topic, string content)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
            INSERT INTO ""Posts"" (""UserId"", ""Topic"", ""Content"")
            VALUES (@userId, @topic, @content);
            ";
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@topic", topic);
            command.Parameters.AddWithValue("@content", content);
            command.ExecuteNonQuery();
        }
    }*/

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
    /* return object Post -> read it at ShowPosts.cshtml.cs -> ShowPosts.cshtml shows user 10 recent posts.
      */
    /*public static List<Post> ReadPost()
    {
        List<Post> postList = new();
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
                @"SELECT p.""PostId"", u.""Username"", p.""Topic"", p.""Content"", p.""CreatedAt""
                    FROM ""Posts"" p, ""Users"" u
                    WHERE p.""UserId"" = u.""Id""
                    ORDER BY p.""CreatedAt"" DESC;
                 ";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
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
    }*/

    public async Task<List<Post>> ReadPostAsync()
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

    /*   public static Post ShowPost(int postId)
       {
           using (var connection = new NpgsqlConnection(_connectionString))
           {
               connection.Open();
               var command = connection.CreateCommand();
               command.CommandText =
                   @"
               SELECT p.""PostId"", u.""Username"", p.""Topic"", p.""Content"", p.""CreatedAt""
               FROM ""Posts"" p
               JOIN ""Users"" u ON p.""UserId"" = u.""Id""
               WHERE p.""PostId"" = @postId";
   
               command.Parameters.AddWithValue("@postId", postId);
   
               using (var reader = command.ExecuteReader())
               {
                   if (reader.Read())
                   {
                       return new Post
                       {
                           PostId = reader.GetInt32(0),
                           AuthorName = reader.GetString(1),
                           Topic = reader.GetString(2),
                           Content = reader.GetString(3),
                           CreatedAt = reader.GetDateTime(4),
                       };
                   }
               }
           }
           return null;
       }*/

    public async Task<Post?> ShowPostAsync(int postId)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        await using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText =
                @"
            SELECT p.""PostId"", u.""Username"", p.""Topic"", p.""Content"", p.""CreatedAt""
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
                        Topic = reader.GetString(2),
                        Content = reader.GetString(3),
                        CreatedAt = reader.GetDateTime(4),
                    };
                }
            }
        }
        return null;
    }

    /*
        public static void AddComment(int userId, int postId, int? parentId, string content)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
    
                command.CommandText =
                    @"
                INSERT INTO ""Comments"" (""UserId"", ""PostId"", ""ParentCommentId"", ""Content"")
                VALUES (@userId, @postId, @parentId, @content);
    ";
    
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@postId", postId);
                command.Parameters.AddWithValue("@parentId", (object)parentId ?? DBNull.Value);
                command.Parameters.AddWithValue("@content", content);
    
                command.ExecuteNonQuery();
            }
        }
    */
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
        public static List<Comment> ShowComments(int postId)
        {
                var command = connection.CreateCommand();
                command.CommandText =
                    @"
                 SELECT u.""Username"", c.""ParentCommentId"", c.""CommentedAt"", c.""Content""
                FROM ""Comments"" c
                JOIN ""Users"" u ON c.""UserId"" = u.""Id""
                WHERE c.""PostId"" = @postId
              ";
                command.Parameters.AddWithValue("@postId", postId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
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
                    allComments.Reverse();
                }
                return allComments;
            }
        }
    */
}
