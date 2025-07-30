using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BaseEntites
{
    public class BaseStatusEntity
    {
        public bool IsDeclined { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public bool IsVerified { get; set; } = false;
        public bool IsPending { get; set; } = false;
    }
}
