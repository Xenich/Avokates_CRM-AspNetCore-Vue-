using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebSite.Models.Outputs;

namespace Avokates_CRM.Models.ApiModels
{
    public class BaseResult
    {
        public List<ErrorMessageResult> ErrorMessages { get; set; }
        public string Status { get; set; }
    }

}
