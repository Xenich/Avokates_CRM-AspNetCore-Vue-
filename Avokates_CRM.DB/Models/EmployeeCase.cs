using System;
using System.Collections.Generic;

namespace Avokates_CRM.DB.Models
{
    public partial class EmployeeCase
    {
        public bool IsOwner { get; set; }
        public Guid EmployeeUid { get; set; }
        public Guid CaseUid { get; set; }
        public int Id { get; set; }
        public byte[] EncriptedAesKey { get; set; }

        public virtual Case CaseU { get; set; }
        public virtual Employee EmployeeU { get; set; }
    }
}
