using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.BaseEntites;
using Domain.Models.blog;

namespace Domain.Models.User
{
    public class AppUser : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(500)]
        public string FullName { get; set; } = "";

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(500)]
        public string UserName { get; set; } = "";

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(200)]
        public string Email { get; set; } = "";

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(500)]
        public string Password { get; set; } = "";

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(50)]
        public string Role { get; set; } = "";

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(500)]
        public string ProfileImageUrl { get; set; } = "";

        public bool IsBlocked { get; set; } = false;

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(500)]
        public string BlockedReason { get; set; } = "";

        public bool IsEmailVerified { get; set; } = false;

        public DateTime AddedDate { get; set; } = DateTime.UtcNow;

        public ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
        public ICollection<BlogComment> BlogComments { get; set; } = new List<BlogComment>();
        public ICollection<BlogLike> BlogLikes { get; set; } = new List<BlogLike>();
        public ICollection<BlogSave> BlogSaves { get; set; } = new List<BlogSave>();





    }
}
