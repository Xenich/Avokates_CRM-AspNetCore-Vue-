
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Avokates_CRM.Models.Outputs
{
    public class GetCase_Out : ResultBase
    {
        public Guid CaseUid { get; set; }
        public string Title { get; set; }
        public string Info { get; set; }
        public string DateCreate { get; set; }
        public string UpdateDate { get; set; }
        public bool IsClosed { get; set; }
        public Case_Employee[] EmployeesWithAccess { get; set; }
        public Case_Employee[] EmployeesWithoutAccess { get; set; }
        public Case_Figurant[] Figurants { get; set; }
        public Case_Note[] Notes { get; set; }
        public bool CanManage { get; set; }
        public bool HasAccess { get; set; }
        public ItemView[] FigurantRoleOptions { get; set; }

    }

    public class Case_Figurant
    {
        public Guid Uid { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Phone { get; set; }
    }

    public class Case_Employee
    {
        public string Name { get; set; }
        public bool IsOwner { get; set; }
        public bool CanManageThisEmployee { get; set; }
        public Guid EmployeeUid { get; set; }
    }

    public class Case_Note
    {
        public long Id { get; set; }
        public Guid Uid { get; set; }
        public string EmployeeInfo { get; set; }
        public string Date { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public bool CanDelete { get; set; }
    }
}
