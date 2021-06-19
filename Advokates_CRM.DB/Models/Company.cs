using System;
using System.Collections.Generic;

namespace Advokates_CRM.DB.Models
{
    public partial class Company
    {
        public Company()
        {
            Case = new HashSet<Case>();
            Employee = new HashSet<Employee>();
            FigurantRole = new HashSet<FigurantRole>();
            Invite = new HashSet<Invite>();
        }

        public Guid Uid { get; set; }
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string CompanyDirector { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhone { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<Case> Case { get; set; }
        public virtual ICollection<Employee> Employee { get; set; }
        public virtual ICollection<FigurantRole> FigurantRole { get; set; }
        public virtual ICollection<Invite> Invite { get; set; }
    }
}
