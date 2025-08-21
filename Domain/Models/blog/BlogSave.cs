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
    public class BlogSave
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("BlogPost")]
        public long BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; }

        [ForeignKey("AppUser")]
        public long UserId { get; set; }
        public AppUser User { get; set; }
    }
}
