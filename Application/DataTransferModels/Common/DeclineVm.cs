﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DataTransferModels.Common
{
    public class DeclineVm
    {
        public long Id { get; set; }
        public string Reason { get; set; } = "";
    }
}
