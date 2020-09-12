using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Avokates_CRM.Models.Inputs
{
    public class NewCase_Figurant_In
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string SecondName { get; set; }
        public string Description { get; set; }       
        public string Phone { get; set; }
        public string Email { get; set; }
        public Guid? RoleUid { get; set; }
    }
}
