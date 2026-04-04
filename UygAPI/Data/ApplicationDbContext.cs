using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UygAPI.Models;

namespace UygAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<NewsTag> NewsTags { get; set; }
        public DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // User - News ilişkisi (AuthorId)
            builder.Entity<News>()
                .HasOne(n => n.Author)
                .WithMany(u => u.News)
                .HasForeignKey(n => n.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // News - Category ilişkisi
            builder.Entity<News>()
                .HasOne(n => n.Category)
                .WithMany(c => c.News)
                .HasForeignKey(n => n.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // News - Comment ilişkisi
            builder.Entity<Comment>()
                .HasOne(c => c.News)
                .WithMany(n => n.Comments)
                .HasForeignKey(c => c.NewsId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - Comment ilişkisi
            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // News - Like ilişkisi
            builder.Entity<Like>()
                .HasOne(l => l.News)
                .WithMany(n => n.Likes)
                .HasForeignKey(l => l.NewsId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - Like ilişkisi
            builder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique constraint for Like (bir kullanıcı bir haberi sadece bir kez beğenebilir)
            builder.Entity<Like>()
                .HasIndex(l => new { l.NewsId, l.UserId })
                .IsUnique();

            // NewsTag composite primary key
            builder.Entity<NewsTag>()
                .HasKey(nt => new { nt.NewsId, nt.TagId });

            // Category Name unique
            builder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();

            // Tag Name unique
            builder.Entity<Tag>()
                .HasIndex(t => t.Name)
                .IsUnique();

            // Seed Data - Default Roles
            builder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", NormalizedName = "ADMIN", Description = "Sistem Yöneticisi", Slug = "admin", CreatedAt = DateTime.UtcNow, IsActive = true },
                new Role { Id = 2, Name = "Editor", NormalizedName = "EDITOR", Description = "Haber Editörü", Slug = "editor", CreatedAt = DateTime.UtcNow, IsActive = true },
                new Role { Id = 3, Name = "User", NormalizedName = "USER", Description = "Normal Kullanıcı", Slug = "user", CreatedAt = DateTime.UtcNow, IsActive = true }
            );

            // Seed Data - Default Categories
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Gündem", Slug = "gundem", Description = "Güncel haberler", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Category { Id = 2, Name = "Teknoloji", Slug = "teknoloji", Description = "Teknoloji haberleri", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Category { Id = 3, Name = "Spor", Slug = "spor", Description = "Spor haberleri", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Category { Id = 4, Name = "Ekonomi", Slug = "ekonomi", Description = "Ekonomi haberleri", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Category { Id = 5, Name = "Magazin", Slug = "magazin", Description = "Magazin haberleri", IsActive = true, CreatedAt = DateTime.UtcNow }
            );
        }
    }
}