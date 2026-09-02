using Microsoft.EntityFrameworkCore;
using toad.Data.Entities;

namespace toad.Data;

public class ApplicationDbContext : DbContext
{
    // Konstruktor pro předání konfigurace (connection stringu) z Program.cs
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<PostEntity> Posts { get; set; } = null!;
    public DbSet<CommentEntity> Comments { get; set; } = null!;

    // Kompletní mapování databáze (Fluent API)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ==========================================
        // CONFIG: USERS
        // ==========================================
        modelBuilder.Entity<UserEntity>().HasKey(u => u.Id);

        modelBuilder.Entity<UserEntity>().HasIndex(u => u.Username).IsUnique();

        modelBuilder
            .Entity<UserEntity>()
            .Property(u => u.JoinDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // ==========================================
        // CONFIG: POSTS
        // ==========================================
        // Tímto řádkem explicitně říkáme EF Core, že klíč je "PostId"
        modelBuilder.Entity<PostEntity>().HasKey(p => p.PostId);

        modelBuilder
            .Entity<PostEntity>()
            .Property(p => p.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Cizí klíč do tabulky Users (ON DELETE CASCADE)
        modelBuilder
            .Entity<PostEntity>()
            .HasOne(p => p.User)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ==========================================
        // CONFIG: COMMENTS
        // ==========================================
        // Tímto řádkem explicitně říkáme EF Core, že klíč je "CommentId"
        modelBuilder.Entity<CommentEntity>().HasKey(c => c.CommentId);

        modelBuilder
            .Entity<CommentEntity>()
            .Property(c => c.CommentedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Cizí klíč do tabulky Users (ON DELETE CASCADE)
        modelBuilder
            .Entity<CommentEntity>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Cizí klíč do tabulky Posts (ON DELETE CASCADE)
        modelBuilder
            .Entity<CommentEntity>()
            .HasOne<PostEntity>()
            .WithMany()
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        // Self-referencing cizí klíč pro pod-komentáře (ON DELETE CASCADE)
        modelBuilder
            .Entity<CommentEntity>()
            .HasOne<CommentEntity>()
            .WithMany()
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
