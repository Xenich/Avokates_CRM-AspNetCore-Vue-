using System;
using System.Collections.Generic;

namespace Avokates_CRM.Models.DB
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
        public string Info { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsClosed { get; set; }
        public int IdPerCompany { get; set; }

        public Company CompanyU { get; set; }
        public ICollection<EmployeeCase> EmployeeCase { get; set; }
        public ICollection<Figurant> Figurant { get; set; }
        public ICollection<Note> Note { get; set; }
    }
}
