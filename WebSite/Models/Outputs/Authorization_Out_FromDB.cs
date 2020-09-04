using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSite.Models.Outputs
{
    public class Authorization_Out_FromDB
    {
        public int EmployeeId { get; set; }
        public Guid EmployeeUid { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string RoleName { get; set; }
        public bool? IsActive { get; set; }
        public Guid CompanyUID { get; set; }

        public string Token { get; set; }
    }
}
