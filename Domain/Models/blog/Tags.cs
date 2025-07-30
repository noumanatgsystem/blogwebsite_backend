using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.BaseEntites;

namespace Domain.Models.blog
{
    public class Tags : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column(TypeName = "VARCHAR")]
        [MaxLength(255)]
        public string Name { get; set; } = "";
    }
}
