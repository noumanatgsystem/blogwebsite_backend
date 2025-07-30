using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.blog
{
    public class BlogPostTagRel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("BlogPost")]
        public long BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; }

        [ForeignKey("Tags")]
        public long TagId { get; set; }
        public Category Tags { get; set; }
    }
}
