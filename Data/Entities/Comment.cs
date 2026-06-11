using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace toad.Data.Entities
{
    public class CommentEntity
    {
        public int CommentId { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public int? ParentCommentId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CommentedAt { get; set; }

        public UserEntity User { get; set; } = null!;
    }
}
