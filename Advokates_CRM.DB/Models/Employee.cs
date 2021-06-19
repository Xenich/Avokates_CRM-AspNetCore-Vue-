using System;
using System.Collections.Generic;

namespace Advokates_CRM.DB.Models
{
    public partial class Employee
    {
        public Employee()
        {
            Auth = new HashSet<Auth>();
            EmployeeCase = new HashSet<EmployeeCase>();
            Note = new HashSet<Note>();
        }

        public Guid Uid { get; set; }
        public int Id { get; set; }
        public Guid CompanyUid { get; set; }
        public Guid? ClassificatorUid { get; set; }
        public Guid? RoleUid { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string SecondName { get; set; }
        public DateTime? Birthday { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public bool? IsActive { get; set; }
        public byte[] PublicKey { get; set; }

        public virtual Classificator ClassificatorU { get; set; }
        public virtual Company CompanyU { get; set; }
        public virtual Role RoleU { get; set; }
        public virtual ICollection<Auth> Auth { get; set; }
        public virtual ICollection<EmployeeCase> EmployeeCase { get; set; }
        public virtual ICollection<Note> Note { get; set; }
    }
}
