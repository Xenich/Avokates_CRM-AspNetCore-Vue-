using System;
using System.Collections.Generic;

namespace Advokates_CRM.DB.Models
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

        public virtual Company CompanyU { get; set; }
        public virtual ICollection<Figurant> Figurant { get; set; }
    }
}
