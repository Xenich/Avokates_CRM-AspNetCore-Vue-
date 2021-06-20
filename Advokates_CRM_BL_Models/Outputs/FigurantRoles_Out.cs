using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advokates_CRM_DTO.Outputs
{
    public class FigurantRoles_Out : ResultBase
    {
        public Dictionary<Guid, string> figurantRolesDictionary { get; set; }
    }
}
