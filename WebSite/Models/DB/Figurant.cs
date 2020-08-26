using System;
using System.Collections.Generic;

namespace Avokates_CRM.Models.DB
{
    public partial class Figurant
    {
        public int Id { get; set; }
        public Guid CaseUid { get; set; }
        public Guid? FigurantRoleUid { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string FigurantRoleName { get; set; }

        public Case CaseU { get; set; }
        public FigurantRole FigurantRoleU { get; set; }
    }
}
