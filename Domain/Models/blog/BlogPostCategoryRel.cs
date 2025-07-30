using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.blog
{
    public class BlogPostCategoryRel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("BlogPost")]
        public long BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; }

        [ForeignKey("Category")]
        public long CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
