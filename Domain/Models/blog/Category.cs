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
    public class Category : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(500)]
        public string Title { get; set; } = "";

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(500)]
        public string Slug { get; set; } = "";

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(1000)]
        public string Description { get; set; } = "";
    }
}
