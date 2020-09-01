using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSite.Models.Outputs
{
    public class GetCabinetInfo_Out : ResultBase
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string SecondName { get; set; }
        public string PublicKey { get; set; }
    }
}
