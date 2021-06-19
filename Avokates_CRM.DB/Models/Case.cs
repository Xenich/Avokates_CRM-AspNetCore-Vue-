using System;
using System.Collections.Generic;

namespace Avokates_CRM.DB.Models
{
    public partial class Case
    {
        public Case()
        {
            EmployeeCase = new HashSet<EmployeeCase>();
            Figurant = new HashSet<Figurant>();
            Note = new HashSet<Note>();
        }

        public Guid Uid { get; set; }
        public Guid CompanyUid { get; set; }
        public string Title { get; set; }
        public byte[] Info { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsClosed { get; set; }
        public int IdPerCompany { get; set; }

        public virtual Company CompanyU { get; set; }
        public virtual ICollection<EmployeeCase> EmployeeCase { get; set; }
        public virtual ICollection<Figurant> Figurant { get; set; }
        public virtual ICollection<Note> Note { get; set; }
    }
}
