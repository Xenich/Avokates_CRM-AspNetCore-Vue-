using System;
using System.Collections.Generic;

namespace Avokates_CRM.DB.Models
{
    public partial class Role
    {
        public Role()
        {
            Employee = new HashSet<Employee>();
        }

        public Guid Uid { get; set; }
        public string RoleName { get; set; }

        public virtual ICollection<Employee> Employee { get; set; }
    }
}
