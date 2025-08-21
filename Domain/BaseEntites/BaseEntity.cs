using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BaseEntites
{
    public class BaseEntity
    {
        public bool IsDeleted { get; set; } = false;
        public bool IsDeclined { get; set; } = false;
        public bool IsVerified { get; set; } = false;
        public bool IsPending { get; set; } = false;

        public long createdBy { get;set; }
        public long updatedBy { get;set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
