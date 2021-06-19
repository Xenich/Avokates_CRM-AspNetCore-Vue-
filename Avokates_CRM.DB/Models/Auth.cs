using System;
using System.Collections.Generic;

namespace Avokates_CRM.DB.Models
{
    public partial class Auth
    {
        public string Login { get; set; }
        public string PassHash { get; set; }
        public string Salt { get; set; }
        public Guid EmployeeUid { get; set; }
        public int Id { get; set; }

        public virtual Employee EmployeeU { get; set; }
    }
}
