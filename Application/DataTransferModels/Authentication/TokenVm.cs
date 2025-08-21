using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DataTransferModels.Authentication
{
    public class TokenVm
    {
        public static long UserID { get; set; }
        public static string UserEmail { get; set; } = "";
        public static string UserName { get; set; } = "";
        public static string Role { get; set; } = "";
    }
}
