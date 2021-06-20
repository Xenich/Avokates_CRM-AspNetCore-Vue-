
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advokates_CRM_DTO.Outputs
{
    public class Authorization_Out : ResultBase
    {
        public string Name { get; set; }
        public string Token { get; set; }
    }
}
