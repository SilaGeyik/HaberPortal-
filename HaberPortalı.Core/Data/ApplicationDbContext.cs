using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HaberPortal.Core.Entities;
using HaberPortal.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace HaberPortal.Core.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, int,
        IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<NewsImage> NewsImages { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<NewsTag> NewsTags { get; set; }
        public DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========== IDENTITY İLİŞKİLERİ (CASCADE HATASINI ÇÖZEN KISIM) ==========
            // UserRole ilişkisini özelleştir - cascade delete'i kaldır
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });

                entity.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Restrict);  // Cascade değil!

                entity.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);  // Cascade değil!
            });

            // Diğer Identity ilişkilerinde cascade delete'i kaldır
            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys())
                .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade &&
                             (fk.PrincipalEntityType.ClrType == typeof(User) ||
                              fk.PrincipalEntityType.ClrType == typeof(Role))))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // ========== PROJE İLİŞKİLERİ ==========

            // User - News ilişkisi
            modelBuilder.Entity<News>()
                .HasOne(n => n.Author)
                .WithMany(u => u.News)
                .HasForeignKey(n => n.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // News - Category ilişkisi
            modelBuilder.Entity<News>()
                .HasOne(n => n.Category)
                .WithMany(c => c.News)
                .HasForeignKey(n => n.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // News - DefaultImage ilişkisi
            modelBuilder.Entity<News>()
                .HasOne(n => n.DefaultImage)
                .WithMany()
                .HasForeignKey(n => n.DefaultImageId)
                .OnDelete(DeleteBehavior.Restrict);

            // News - Images ilişkisi
            modelBuilder.Entity<NewsImage>()
                .HasOne(ni => ni.News)
                .WithMany(n => n.Images)
                .HasForeignKey(ni => ni.NewsId)
                .OnDelete(DeleteBehavior.Cascade);

            // News - Comments ilişkisi
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.News)
                .WithMany(n => n.Comments)
                .HasForeignKey(c => c.NewsId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - Comments ilişkisi
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // News - Likes ilişkisi
            modelBuilder.Entity<Like>()
                .HasOne(l => l.News)
                .WithMany(n => n.Likes)
                .HasForeignKey(l => l.NewsId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - Likes ilişkisi
            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // NewsTag (Many-to-Many) ilişkisi
            modelBuilder.Entity<NewsTag>()
                .HasKey(nt => new { nt.NewsId, nt.TagId });

            modelBuilder.Entity<NewsTag>()
                .HasOne(nt => nt.News)
                .WithMany(n => n.NewsTags)
                .HasForeignKey(nt => nt.NewsId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NewsTag>()
                .HasOne(nt => nt.Tag)
                .WithMany(t => t.NewsTags)
                .HasForeignKey(nt => nt.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========== INDEX'LER ==========

            // Slug alanları için unique index
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Slug)
                .IsUnique();

            modelBuilder.Entity<News>()
                .HasIndex(n => n.Slug)
                .IsUnique();

            modelBuilder.Entity<Tag>()
                .HasIndex(t => t.Slug)
                .IsUnique();

            // ========== GLOBAL FİLTRELER ==========

            // Sadece aktif kayıtları getir
            modelBuilder.Entity<Category>().HasQueryFilter(c => c.IsActive);
            modelBuilder.Entity<News>().HasQueryFilter(n => n.IsActive);
            modelBuilder.Entity<Tag>().HasQueryFilter(t => t.IsActive);
            modelBuilder.Entity<Comment>().HasQueryFilter(c => c.IsActive);
        }
    }
}