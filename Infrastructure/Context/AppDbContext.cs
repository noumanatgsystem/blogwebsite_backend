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


        #region Users
        public DbSet<AppUser> AppUser { get; set; }

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
