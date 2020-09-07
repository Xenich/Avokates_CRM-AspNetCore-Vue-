using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Avokates_CRM.Models.Outputs
{
    public class Registration_Out : ResultBase
    {
        public string PrivateKey { get; set; }
        public Guid UserUid { get; set; }
        public string JWT { get; set; }
    }
}
