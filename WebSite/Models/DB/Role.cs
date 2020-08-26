using System;
using System.Collections.Generic;

namespace Avokates_CRM.Models.DB
{
    public partial class Role
    {
        public Role()
        {
            Employee = new HashSet<Employee>();
        }

        public Guid Uid { get; set; }
        public string RoleName { get; set; }

        public ICollection<Employee> Employee { get; set; }
    }
}
