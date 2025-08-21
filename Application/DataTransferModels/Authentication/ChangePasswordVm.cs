using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DataTransferModels.Authentication
{
    public class ChangePasswordVm
    {
        public class ChangeUserPasswordVm
        {
            public long UserID { get; set; }
            public string OldPassword { get; set; } = "";
            public string NewPassword { get; set; } = "";
            public string ConfirmPassword { get; set; } = "";
        }
    }
}
