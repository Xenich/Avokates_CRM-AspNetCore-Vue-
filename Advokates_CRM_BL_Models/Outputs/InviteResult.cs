using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advokates_CRM_DTO.Outputs
{
    public class InviteResult: ResultBase
    {
        public string InviteToken { get; set; }
        public string CompanyName { get; set; }
    }
}
