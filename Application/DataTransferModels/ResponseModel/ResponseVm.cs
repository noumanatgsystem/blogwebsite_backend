using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DataTransferModels.ResponseModel
{
    public sealed class ResponseVm
    {
        private static ResponseVm? instance = null;

        private ResponseVm()
        {
        }

        public static ResponseVm Instance
        {
            get
            {
                //if (instance == null)
                {
                    instance = new ResponseVm();
                }
                return instance;
            }
        }
        public int responseCode { get; set; }
        public string errorMessage { get; set; } = "";
        public string responseMessage { get; set; } = "";
        public dynamic data { get; set; }
    }
}
