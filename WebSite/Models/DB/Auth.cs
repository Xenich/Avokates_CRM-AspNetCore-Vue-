using System;
using System.Collections.Generic;

namespace Avokates_CRM.Models.DB
{
    public partial class Auth
    {
        public string Login { get; set; }
        public string PassHash { get; set; }
        public string Salt { get; set; }
        public Guid EmployeeUid { get; set; }
        public int Id { get; set; }

        public Employee EmployeeU { get; set; }
    }
}
