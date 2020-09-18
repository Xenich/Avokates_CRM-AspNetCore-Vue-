using System;
using System.Collections.Generic;

namespace Avokates_CRM.Models.DB
{
    public partial class FigurantRole
    {
        public FigurantRole()
        {
            Figurant = new HashSet<Figurant>();
        }

        public Guid Uid { get; set; }
        public string RoleName { get; set; }
        public bool? IsActive { get; set; }
        public Guid? CompanyUid { get; set; }

        public Company CompanyU { get; set; }
        public ICollection<Figurant> Figurant { get; set; }
    }
}
