using System;
using System.Collections.Generic;

namespace Avokates_CRM.DB.Models
{
    public partial class Classificator
    {
        public Classificator()
        {
            Employee = new HashSet<Employee>();
        }

        public Guid Uid { get; set; }
        public string ProfessionName { get; set; }
        public string ProfessionCode { get; set; }

        public virtual ICollection<Employee> Employee { get; set; }
    }
}
