using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DataTransferModels.Common;

namespace Application.DataTransferModels.AppUser
{
    public class UserSearchVm : PaginationVm
    {
        public string SearchText { get; set; } = "";
        public string GetName { get; set; } = "";
        public string UserType { get; set; } = "";
        public string Gender { get; set; } = "";
    }
}
