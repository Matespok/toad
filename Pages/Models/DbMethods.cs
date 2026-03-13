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
    public static void InitializeDb()
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            //Table Users
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                CREATE TABLE IF NOT EXISTS ""Users"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    ""Username"" TEXT UNIQUE NOT NULL,
                    ""Password"" TEXT NOT NULL
                );";

            command.ExecuteNonQuery();

            //Table posts
            command.CommandText =
                @"
            CREATE TABLE IF NOT EXISTS ""Posts"" (
                ""PostId"" SERIAL PRIMARY KEY,
                ""UserId"" INTEGER NOT NULL,
                ""Topic"" TEXT NOT NULL,
                ""Content"" TEXT NOT NULL,
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                CONSTRAINT fk_user FOREIGN KEY(""UserId"") REFERENCES ""Users""(""Id"") ON DELETE CASCADE
            );";
            command.ExecuteNonQuery();

            //Table comments
            /*
             * Table comments
             * CommentId will create itself,
             * CommentedAt will create itself
             *
             * Int UserId
             * Int PostId
             * Int ParentCommentId -> nullable
             * string content
             * */
            command.CommandText =
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
            command.ExecuteNonQuery();
        }
    }

    /*
     * ==================
     * User registration
     * ==================
     */
    public static void AddUser(string username, string password)
    {
        string passHash = BCrypt.Net.BCrypt.HashPassword(password);
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                INSERT INTO ""Users"" (""Username"", ""Password"") 
                VALUES (@uname, @pass);";
            command.Parameters.AddWithValue("@uname", username);
            command.Parameters.AddWithValue("@pass", passHash);

            command.ExecuteNonQuery();
        }
    }

    /*
     * =============
     * USER LOGIN
     * =============
    */
    public static int? AuthenticateUser(string username, string password)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            var command = connection.CreateCommand();

            command.CommandText =
                @"
            SELECT ""Id"", ""Password"" 
            FROM ""Users"" 
            WHERE ""Username"" = @uname;";
            command.Parameters.AddWithValue("@uname", username);

            using (var reader = command.ExecuteReader())
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
    }

    /*
     * =============
     * Post creation
     * =============
    */
    public static void AddPost(int? userId, string topic, string content)
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
    }

    /*
     * ===========
     * post read
     * ===========
     * */
    /* return object Post -> read it at ShowPosts.cshtml.cs -> ShowPosts.cshtml shows user 10 recent posts.
      */
    public static List<Post> ReadPost()
    {
        List<Post> postList = new();
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
                @"select p.""PostId"", u.""Username"", p.""Topic"", p.""Content"", p.""CreatedAt""
                    from ""Posts"" p, ""Users"" u
                    WHERE p.""UserId"" = u.""Id"";
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

                postList.Reverse();
            }
            return postList;
        }
    }

    public static Post ShowPost(int postId)
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
    }

    //Table comments
    /*
     * Table comments
     * CommentId will create itself,
     *
     * Int UserId
     * Int PostId
     * Int ParentCommentId -> nullable
     * string content
     * */

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

    public static List<Comment> ShowComments(int postId)
    {
        List<Comment> allComments = new();
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            /* Structure of class Comments ->
               *    public string AuthorName { get; set; }
                    public int ParentCommentId { get; set; }
                    public DateTime CommentedAt { get; set; }
                    string Content { get; set; }
              */
            var command = connection.CreateCommand();
            command.CommandText =
                @"
             SELECT u.""Username"", c.""ParentCommentId"", c.""CommentedAt"", c.""Content""
            FROM ""Comments"" c
            JOIN ""Users"" u ON c.""UserId"" = u.""Id""  -- Předpokládám název sloupce UserId a Id
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
}
