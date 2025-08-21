using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DataTransferModels.Authentication
{
    public class RefreshTokenVm
    {
        public string Token { get; set; } = "";
        public string RefreshToken { get; set; } = "";
    }
}
