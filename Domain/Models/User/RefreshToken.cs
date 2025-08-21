using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.User
{
    public class RefreshToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Column(TypeName = "NVARCHAR")]
        [MaxLength(400)]
        public string Token { get; set; }

        public DateTime Expires { get; set; }

        [NotMapped]
        public bool IsExpired => DateTime.UtcNow >= Expires;

        [ForeignKey("AppUser")]
        public long UserId { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
