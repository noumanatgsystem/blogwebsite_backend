using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DataTransferModels.AppUser
{
    public class LoginUserVM
    {
        public string Mail { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
