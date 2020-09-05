using Avokates_CRM.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSite.Models.Outputs
{
    public class Authorization_Out : ResultBase
    {
        public string Name { get; set; }
        public string Token { get; set; }
    }
}
