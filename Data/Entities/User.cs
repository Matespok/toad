using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace toad.Data.Entities
{
    public class UserEntity
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }

        // Navigační vlastnosti
        public ICollection<PostEntity> Posts { get; set; } = new List<PostEntity>();
        public ICollection<CommentEntity> Comments { get; set; } = new List<CommentEntity>();
    }
}
