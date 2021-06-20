using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advokates_CRM_DTO.Outputs
{
    public class Registration_Out : ResultBase
    {
        public string PrivateKey { get; set; }
        public Guid UserUid { get; set; }
        public string JWT { get; set; }
    }
}
