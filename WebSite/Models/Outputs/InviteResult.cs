using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Avokates_CRM.Models.Outputs
{
    public class InviteResult: ResultBase
    {
        public string InviteToken { get; set; }
        public string CompanyName { get; set; }
    }
}
