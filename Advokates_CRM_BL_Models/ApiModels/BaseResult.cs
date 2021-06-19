using Advokates_CRM_BL_Models.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advokates_CRM_BL_Models.ApiModels
{
    public class BaseResult
    {
        public List<ErrorMessageResult> ErrorMessages { get; set; }
        public string Status { get; set; }
    }

}
