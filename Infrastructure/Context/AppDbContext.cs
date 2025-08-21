using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.blog;
using Domain.Models.User;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // BlogPost → BlogComment (Cascade)
            modelBuilder.Entity<BlogComment>()
                .HasOne(c => c.BlogPost)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);

            // BlogPost → BlogLike (Cascade)
            modelBuilder.Entity<BlogLike>()
                .HasOne(l => l.BlogPost)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);

            // BlogPost → BlogSave (Cascade)
            modelBuilder.Entity<BlogSave>()
                .HasOne(s => s.BlogPost)
                .WithMany(p => p.Saves)
                .HasForeignKey(s => s.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);

            // AppUser → BlogPost (Cascade)
            modelBuilder.Entity<BlogPost>()
                .HasOne(p => p.AppUser)
                .WithMany(u => u.BlogPosts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // AppUser → BlogComment (Restrict to avoid cycle)
            modelBuilder.Entity<BlogComment>()
                .HasOne(c => c.AppUser)
                .WithMany(u => u.BlogComments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // AppUser → BlogLike (Restrict to avoid cycle)
            modelBuilder.Entity<BlogLike>()
                .HasOne(l => l.User)
                .WithMany(u => u.BlogLikes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // AppUser → BlogSave (Restrict to avoid cycle)
            modelBuilder.Entity<BlogSave>()
                .HasOne(s => s.User)
                .WithMany(u => u.BlogSaves)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

        }



        #region Users
        public DbSet<AppUser> AppUser { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }

        #endregion

        #region Blog 
        public DbSet<BlogPost> BlogPost { get; set; }
        public DbSet<BlogPostCategoryRel> BlogPostCategoryRel { get; set; }
        public DbSet<BlogPostTagRel> BlogPostTagRel { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<BlogComment> BlogComment { get; set; }
        public DbSet<BlogLike> BlogLike { get; set; }
        public DbSet<BlogSave> BlogSave { get; set; }
        public DbSet<Tags> Tags { get; set; }

        #endregion
    }
}
