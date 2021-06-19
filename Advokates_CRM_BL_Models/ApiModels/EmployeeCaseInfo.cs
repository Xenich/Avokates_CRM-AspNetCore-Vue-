using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advokates_CRM_BL_Models.ApiModels
{
    public class EmployeeCaseInfo
    {
        public byte[] encriptedAesKey;
        public bool isOwner;
        public string userRole;
        public Guid employeeGuid;
        public Guid companyUid;
    }
}
