﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DataTransferModels.AppUser
{
    public class LoggedInUser
    {
        public string Token { get; set; } = "";
        public string RefreshToken { get; set; } = "";
        public string UserName { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public string ProfileImageUrl { get; set; } = "";
    }
}
