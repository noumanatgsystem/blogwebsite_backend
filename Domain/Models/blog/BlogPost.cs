using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.User;
using Domain.BaseEntites;

namespace Domain.Models.blog
{
    public class BlogPost : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("AppUser")]
        public long UserId { get; set; }
        public AppUser AppUser { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(500)]
        public string Title { get; set; } = "";

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(500)]
        public string Slug { get; set; } = "";

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(1000)]
        public string Description { get; set; } = "";

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(1000)]
        public string FileUrl { get; set; } = "";

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(1000)]
        public string FileKey { get; set; } = "";

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(1000)]
        public string? StyleUrl { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(1000)]
        public string? StyleKey { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(20)]
        public string Status { get; set; } = "Draft";

        public ICollection<BlogComment> Comments { get; set; } = new List<BlogComment>();

        // Navigation property for likes
        public ICollection<BlogLike> Likes { get; set; } = new List<BlogLike>();

        // Navigation property for saves
        public ICollection<BlogSave> Saves { get; set; } = new List<BlogSave>();

        // Many-to-many with Categories
        public ICollection<BlogPostCategoryRel> BlogPostCategories { get; set; } = new List<BlogPostCategoryRel>();
    }
}
