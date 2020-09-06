using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Avokates_CRM.Models.Outputs
{
    public class FigurantRoles_Out : ResultBase
    {
        public Dictionary<Guid, string> figurantRolesDictionary { get; set; }
    }
}
