using Avokates_CRM.Models.DB;
using Avokates_CRM.Models.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Avokates_CRM.Helpers
{
    public class DBHelper
    {
        public static ItemView[] GetFigurantRoleOptions(Guid companyUid, LawyerCRMContext _context)
        {
            return _context.FigurantRole.Where(f => f.CompanyUid == companyUid).Select(f => new ItemView()
            {
                Id = f.Uid.ToString(),
                Name = f.RoleName
            })
            .OrderBy(f => f.Name)
            .ToArray();

        }
    }
}
