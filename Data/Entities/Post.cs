using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace toad.Data.Entities
{
    public class PostEntity
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Topic { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public UserEntity User { get; set; } = null!;
    }
}
/* CREATE TABLE IF NOT EXISTS ""Posts"" (
                ""PostId"" SERIAL PRIMARY KEY,
                ""UserId"" INTEGER NOT NULL,
                ""Topic"" TEXT NOT NULL,
                ""Content"" TEXT NOT NULL,
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                CONSTRAINT fk_user FOREIGN KEY(""UserId"") REFERENCES ""Users""(""Id"") ON DELETE CASCADE
            );";
*/
