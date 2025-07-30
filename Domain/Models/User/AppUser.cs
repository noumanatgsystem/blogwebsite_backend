using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.BaseEntites;

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

        public bool IsBlocked { get; set; } = false;





    }
}
