using System;
using System.Collections.Generic;

namespace Avokates_CRM.DB.Models
{
    public partial class Invite
    {
        public Guid CompanyUid { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresIn { get; set; }

        public virtual Company CompanyU { get; set; }
    }
}
