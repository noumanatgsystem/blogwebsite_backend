using Domain.Models.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.blog
{
    public class BlogComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("BlogPost")]
        public long BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; }

        [ForeignKey("AppUser")]
        public long UserId { get; set; }
        public AppUser AppUser { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(2000)]
        public string Content { get; set; } = "";

        public bool IsApproved { get; set; } = true;
    }
}
