using System;
using System.Collections.Generic;

namespace Avokates_CRM.Models.DB
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

        public ICollection<Employee> Employee { get; set; }
    }
}
